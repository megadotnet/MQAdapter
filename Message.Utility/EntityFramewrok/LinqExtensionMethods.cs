
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messag.Utility.EntityFramewrok
{

    /// <summary>
    /// LinqExtensionMethods
    /// </summary>
    public static class LinqExtensionMethods
    {
        /// <summary>
        /// Creates the hierarchy.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="allItems">All items.</param>
        /// <param name="parentItem">The parent item.</param>
        /// <param name="idProperty">The identifier property.</param>
        /// <param name="parentIdProperty">The parent identifier property.</param>
        /// <param name="depth">The depth.</param>
        /// <returns></returns>
        private static System.Collections.Generic.IEnumerable<TreeEntity<TEntity>> CreateHierarchy<TEntity, TProperty>
          (IEnumerable<TEntity> allItems, TEntity parentItem,
          Func<TEntity, TProperty> idProperty, Func<TEntity, TProperty> parentIdProperty, int depth) where TEntity : class
        {
            IEnumerable<TEntity> childs;

            if (parentItem == null)
                childs = allItems.Where(i => parentIdProperty(i).Equals(default(TProperty)));
            else
                childs = allItems.Where(i => parentIdProperty(i).Equals(idProperty(parentItem)));

            if (childs.Count() > 0)
            {
                depth++;

                foreach (var item in childs)
                    yield return new TreeEntity<TEntity>()
                    {
                        Entity = item,
                        children = CreateHierarchy<TEntity, TProperty>
                            (allItems, item, idProperty, parentIdProperty, depth),
                        Depth = depth
                    };
            }
        }

        /// <summary>
        /// LINQ IEnumerable AsHierachy() extension method
        /// </summary>
        /// <typeparam name="TEntity">Entity class</typeparam>
        /// <typeparam name="TProperty">Property of entity class</typeparam>
        /// <param name="allItems">Flat collection of entities</param>
        /// <param name="idProperty">Reference to Id/Key of entity</param>
        /// <param name="parentIdProperty">Reference to parent Id/Key</param>
        /// <returns>Hierarchical structure of entities</returns>
        public static System.Collections.Generic.IEnumerable<TreeEntity<TEntity>> AsHierarchy<TEntity, TProperty>
          (this IEnumerable<TEntity> allItems, Func<TEntity, TProperty> idProperty, Func<TEntity, TProperty> parentIdProperty)
          where TEntity : class
        {
            return CreateHierarchy(allItems, default(TEntity), idProperty, parentIdProperty, 0);
        }

    }
}
