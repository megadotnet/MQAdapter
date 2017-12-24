using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messag.Utility.Transfer
{
    /// <summary>
    /// Interface of TypeAdapter
    /// </summary>
    /// <typeparam name="S">Source</typeparam>
    /// <typeparam name="T">Target</typeparam>
    public interface ITypeAdapter
    {
        /// <summary>
        /// Transforms the specified source.
        /// </summary>
        /// <typeparam name="S">Source type</typeparam>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="source">The source instance</param>
        /// <returns>Target type</returns>
        T Transform<S, T>(S source);
    }

    /// <summary>
    /// AutoMapperTypeAdapter
    /// </summary>
    /// <typeparam name="S">Source</typeparam>
    /// <typeparam name="T">Target</typeparam>
    /// <see cref="https://lostechies.com/jimmybogard/2016/01/21/removing-the-static-api-from-automapper/"/>
    public class AutoMapperTypeAdapter : ITypeAdapter
    {
        private MapperConfiguration mapperConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeAdapter&lt;S, T&gt;"/> class.
        /// </summary>
        public AutoMapperTypeAdapter()
        {
            //We do not need complete mapping between entity and DTO
            //Mapper.AssertConfigurationIsValid();
        }

        /// <summary>
        /// Transforms the specified source.
        /// </summary>
        /// <typeparam name="S">Source type</typeparam>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="source">The source instance</param>
        /// <returns>Target type</returns>
        public virtual T Transform<S, T>(S source)
        {
            CreateMap<S, T>();
            return GetTarget<S, T>(source);
        }

        /// <summary>
        /// Transforms the specified source.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public virtual T[] Transform<S, T>(S[] source)
        {
            var uIList = new List<T>();
            source.ToList().ForEach(e =>
            {
                uIList.Add(Transform<S, T>(e));
            });

            return uIList.ToArray();
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        protected virtual T GetTarget<S, T>(S source)
        {
            IMapper mapper = mapperConfiguration.CreateMapper();
            return mapper.Map<S, T>(source);
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="dest">The dest.</param>
        /// <returns></returns>
        protected virtual T GetTarget<S, T>(S source, T dest)
        {
            IMapper mapper = mapperConfiguration.CreateMapper();
            return mapper.Map<S, T>(source, dest);
        }

        /// <summary>
        /// Creates the map.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        protected virtual void CreateMap<S, T>()
        {
            mapperConfiguration = new MapperConfiguration(cfg => {
                cfg.CreateMap<S, T>();
            });
        }

    }
}
