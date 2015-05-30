using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Messag.Utility.EntLib.Attribute
{
    /// <summary>
    /// An <see cref="ICallHandler"/> that wraps the next handler with a TransactionScope
    /// </summary>
    public class TransactionScopeCallHandler : ICallHandler
    {
        #region Constants and Fields

        /// <summary>
        /// The complete.
        /// </summary>
        private bool complete;

        /// <summary>
        /// The interop option.
        /// </summary>
        private EnterpriseServicesInteropOption interopOption;

        /// <summary>
        /// The transaction options.
        /// </summary>
        private TransactionOptions transactionOptions;

        /// <summary>
        /// The transaction scope option.
        /// </summary>
        private TransactionScopeOption transactionScopeOption;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeCallHandler"/> class. 
        /// Creates a new <see cref="TransactionScopeCallHandler"/>.
        /// </summary>
        /// <param name="transactionScopeOption">
        /// An instance of the TransactionScopeOption enumeration that describes the transaction requirements associated with this transaction scope.
        /// </param>
        /// <param name="transactionOptions">
        /// A TransactionOptions structure that describes the transaction options to use if a new transaction is created. If an existing transaction is used, the timeout value in this parameter applies to the transaction scope. If that time expires before the scope is disposed, the transaction is aborted.
        /// </param>
        /// <param name="interopOption">
        /// An instance of the EnterpriseServicesInteropOption enumeration that describes how the associated transaction interacts with COM+ transactions.
        /// </param>
        /// <param name="complete">
        /// Whether the Transaction should be completed when the next handler executed without exceptions.
        /// </param>
        public TransactionScopeCallHandler(
            TransactionScopeOption transactionScopeOption,
            TransactionOptions transactionOptions,
            EnterpriseServicesInteropOption interopOption,
            bool complete)
        {
            this.transactionScopeOption = transactionScopeOption;
            this.transactionOptions = transactionOptions;
            this.interopOption = interopOption;
            this.complete = complete;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeCallHandler"/> class.
        /// </summary>
        /// <param name="transactionScopeOption">
        /// An instance of the TransactionScopeOption enumeration that describes the transaction requirements associated with this transaction scope.
        /// </param>
        /// <param name="transactionOptions">
        /// A TransactionOptions structure that describes the transaction options to use if a new transaction is created. If an existing transaction is used, the timeout value in this parameter applies to the transaction scope. If that time expires before the scope is disposed, the transaction is aborted.
        /// </param>
        /// <param name="interopOption">
        /// An instance of the EnterpriseServicesInteropOption enumeration that describes how the associated transaction interacts with COM+ transactions.
        /// </param>
        /// <param name="complete">
        /// Whether the Transaction should be completed when the next handler executed without exceptions.
        /// </param>
        /// <param name="order">
        /// Order in which handler will be executed.
        /// </param>
        public TransactionScopeCallHandler(
            TransactionScopeOption transactionScopeOption,
            TransactionOptions transactionOptions,
            EnterpriseServicesInteropOption interopOption,
            bool complete,
            int order)
            : this(transactionScopeOption, transactionOptions, interopOption, complete)
        {
            this.Order = order;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TransactionScopeCallHandler"/> is complete.
        /// </summary>
        /// <value><c>true</c> if complete; otherwise, <c>false</c>.</value>
        public bool Complete
        {
            get
            {
                return this.complete;
            }

            set
            {
                this.complete = value;
            }
        }

        /// <summary>
        /// Gets or sets the enterprise services interop option.
        /// </summary>
        /// <value>The enterprise services interop option.</value>
        public EnterpriseServicesInteropOption InteropOption
        {
            get
            {
                return this.interopOption;
            }

            set
            {
                this.interopOption = value;
            }
        }

        /// <summary>
        /// Order in which the handler will be executed
        /// </summary>
        /// <value></value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the transaction options.
        /// </summary>
        /// <value>The transaction options.</value>
        public TransactionOptions TransactionOptions
        {
            get
            {
                return this.transactionOptions;
            }

            set
            {
                this.transactionOptions = value;
            }
        }

        /// <summary>
        /// Gets or sets the transaction scope option.
        /// </summary>
        /// <value>The transaction scope option.</value>
        public TransactionScopeOption TransactionScopeOption
        {
            get
            {
                return this.transactionScopeOption;
            }

            set
            {
                this.transactionScopeOption = value;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region ICallHandler

        /// <summary>
        /// Implement this method to execute your handler processing.
        /// </summary>
        /// <param name="input">
        /// Inputs to the current call to the target.
        /// </param>
        /// <param name="getNext">
        /// Delegate to execute to get the next delegate in the handler
        /// chain.
        /// </param>
        /// <returns>
        /// Return value from the target.
        /// </returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            IMethodReturn result = null;

            using (TransactionScope scope = this.CreateTransactionScope())
            {
                result = getNext()(input, getNext);

                if (this.complete && result.Exception == null)
                {
                    scope.Complete();
                }
            }

            return result;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Creates the transaction scope.
        /// </summary>
        /// <returns>
        /// </returns>
        protected virtual TransactionScope CreateTransactionScope()
        {
            if (this.interopOption == EnterpriseServicesInteropOption.None)
            {
                return new TransactionScope(this.transactionScopeOption, this.transactionOptions);
            }
            else
            {
                return new TransactionScope(this.transactionScopeOption, this.transactionOptions, this.interopOption);
            }
        }

        #endregion
    }

    /// <summary>
    /// An attribute used to apply the <see cref="TransactionScopeCallHandler"/> to the target.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class TransactionScopeCallHandlerAttribute : HandlerAttribute
    {
        #region Constants and Fields

        /// <summary>
        /// The time span converter.
        /// </summary>
        private static readonly TimeSpanConverter timeSpanConverter = new TimeSpanConverter();

        /// <summary>
        /// The complete.
        /// </summary>
        private bool complete = true;

        /// <summary>
        /// The interop option.
        /// </summary>
        private EnterpriseServicesInteropOption interopOption = EnterpriseServicesInteropOption.None;

        /// <summary>
        /// The isolation level.
        /// </summary>
        private IsolationLevel isolationLevel = IsolationLevel.Serializable;

        /// <summary>
        /// The timeout.
        /// </summary>
        private TimeSpan timeout = TimeSpan.FromMinutes(1);

        /// <summary>
        /// The transaction scope option.
        /// </summary>
        private TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether the handler will complete the TransactionScope on success.
        /// </summary>
        /// <value><c>true</c> if complete; otherwise, <c>false</c>.</value>
        public bool Complete
        {
            get
            {
                return this.complete;
            }

            set
            {
                this.complete = value;
            }
        }

        /// <summary>
        /// Gets or sets the Interop Option used by the handler.
        /// </summary>
        /// <value>Interop Option.</value>
        public EnterpriseServicesInteropOption InteropOption
        {
            get
            {
                return this.interopOption;
            }

            set
            {
                this.interopOption = value;
            }
        }

        /// <summary>
        /// Gets or sets the Isolation Level used by the handler.
        /// </summary>
        /// <value>Scope Option.</value>
        public IsolationLevel IsolationLevel
        {
            get
            {
                return this.isolationLevel;
            }

            set
            {
                this.isolationLevel = value;
            }
        }

        /// <summary>
        /// Gets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout
        {
            get
            {
                return this.timeout;
            }
        }

        /// <summary>
        /// Gets or sets the timeout string.
        /// </summary>
        /// <value>The timeout string.</value>
        public string TimeoutString
        {
            get
            {
                return timeSpanConverter.ConvertToString(this.timeout);
            }

            set
            {
                this.timeout = (TimeSpan)timeSpanConverter.ConvertFromString(value);
            }
        }

        /// <summary>
        /// Gets or sets the Transaction Scope Option used by the handler.
        /// </summary>
        /// <value>Transaction Scope Option.</value>
        public TransactionScopeOption TransactionScopeOption
        {
            get
            {
                return this.transactionScopeOption;
            }

            set
            {
                this.transactionScopeOption = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Derived classes implement this method. When called, it
        /// creates a new call handler as specified in the attribute
        /// configuration.
        /// </summary>
        /// <param name="container">
        /// The <see cref="T:Microsoft.Practices.Unity.IUnityContainer"/> to use when creating handlers,
        /// if necessary.
        /// </param>
        /// <returns>
        /// A new call handler object.
        /// </returns>
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            var transactionOptions = new TransactionOptions();
            transactionOptions.Timeout = this.Timeout;
            transactionOptions.IsolationLevel = this.IsolationLevel;
            return new TransactionScopeCallHandler(
                this.TransactionScopeOption, transactionOptions, this.InteropOption, this.Complete, this.Order);
        }

        #endregion
    }
}
