using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nop.Core.Domain.Customers.CustomerReminder.ReminderCondition;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer action
    /// </summary>
    public partial class CustomerAction : BaseEntity
    {
        private ICollection<ActionCondition> _condition;
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets active action
        /// </summary>
        public bool Active { get; set; }


        /// <summary>
        /// Gets or sets the action Type
        /// </summary>
        public int ActionTypeId { get; set; }

        /// <summary>
        /// Gets or sets the action conditions
        /// </summary>
        public int ConditionId { get; set; }

        public CustomerActionConditionEnum Condition
        {
            get { return (CustomerActionConditionEnum)ConditionId; }
            set { this.ConditionId = (int)value; }
        }


        /// <summary>
        /// Gets or sets the action conditions
        /// </summary>
        public int ReactionTypeId { get; set; }

        public CustomerReactionTypeEnum ReactionType
        {
            get { return (CustomerReactionTypeEnum)ReactionTypeId; }
            set { this.ReactionTypeId = (int)value; }
        }

        public int BannerId { get; set; }
        public int InteractiveFormId { get; set; }

        public int MessageTemplateId { get; set; }

        public int CustomerRoleId { get; set; }

        public int CustomerTagId { get; set; }
        /// <summary>
        /// Gets or sets the start date 
        /// </summary>
        public DateTime StartDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the end date
        /// </summary>
        public DateTime EndDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the customer roles
        /// </summary>
        public virtual ICollection<ActionCondition> Conditions
        {
            get { return _condition ?? (_condition = new List<ActionCondition>()); }
            protected set { _condition = value; }
        }

        public partial class ActionCondition : BaseEntity
        {
            private ICollection<ActionConditionEntity> _entities;
            private ICollection<ActionConditionProductAttributeValue> _productAttribute;
            private ICollection<ActionConditionProductSpecification> _productSpecification;
            private ICollection<CustomerRegister> _customerRegister;
            private ICollection<CustomerRegister> _customCustomerAttributes;
            private ICollection<Url> _urlReferrer;
            private ICollection<Url> _urlCurrent;

            public string Name { get; set; }

            public int CustomerActionConditionTypeId { get; set; }

            public CustomerActionConditionTypeEnum CustomerActionConditionType
            {
                get { return (CustomerActionConditionTypeEnum)CustomerActionConditionTypeId; }
                set { this.CustomerActionConditionTypeId = (int)value; }
            }

            public int ConditionId { get; set; }

            public CustomerActionConditionEnum Condition
            {
                get { return (CustomerActionConditionEnum)ConditionId; }
                set { this.ConditionId = (int)value; }
            }

            public virtual ICollection<ActionConditionEntity> Entity
            {
                get { return _entities ?? (_entities = new List<ActionConditionEntity>()); }
                protected set { _entities = value; }
            }
            
            public virtual ICollection<ActionConditionProductAttributeValue> ProductAttribute
            {
                get { return _productAttribute ?? (_productAttribute = new List<ActionConditionProductAttributeValue>()); }
                protected set { _productAttribute = value; }
            }

            public virtual ICollection<ActionConditionProductSpecification> ProductSpecifications
            {
                get { return _productSpecification ?? (_productSpecification = new List<ActionConditionProductSpecification>()); }
                protected set { _productSpecification = value; }
            }

            public virtual ICollection<CustomerRegister> CustomerRegistration
            {
                get { return _customerRegister ?? (_customerRegister = new List<CustomerRegister>()); }
                protected set { _customerRegister = value; }
            }

            public virtual ICollection<CustomerRegister> CustomCustomerAttributes
            {
                get { return _customCustomerAttributes ?? (_customCustomerAttributes = new List<CustomerRegister>()); }
                protected set { _customCustomerAttributes = value; }
            }

            public virtual ICollection<Url> UrlReferrer
            {
                get { return _urlReferrer ?? (_urlReferrer = new List<Url>()); }
                protected set { _urlReferrer = value; }
            }

            public virtual ICollection<Url> UrlCurrent
            {
                get { return _urlCurrent ?? (_urlCurrent = new List<Url>()); }
                protected set { _urlCurrent = value; }
            }

            public partial class ActionConditionProductSpecification : BaseEntity
            {
                public int ProductSpecificationId { get; set; }
                public int ProductSpecificationValueId { get; set; }
            }

            public partial class ActionConditionProductAttributeValue : BaseEntity
            {
                public int ProductAttributeId { get; set; }
                public string Name { get; set; }
            }

            public partial class Url : BaseEntity
            {
                public string Name { get; set; }
            }
        }
    }
}
