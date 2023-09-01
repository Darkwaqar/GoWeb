using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer reminder 
    /// </summary>
    public partial class CustomerReminder : BaseEntity
    {
        private ICollection<ReminderCondition> _condition;

        private ICollection<ReminderLevel> _level;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        public DateTime StartDateTimeUtc { get; set; }
        public DateTime EndDateTimeUtc { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public bool AllowRenew { get; set; }
        public int RenewedDay { get; set; }
        /// <summary>
        /// Gets or sets display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets active action
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the action conditions
        /// </summary>
        public int ConditionId { get; set; }

        public CustomerReminderConditionEnum Condition
        {
            get { return (CustomerReminderConditionEnum)ConditionId; }
            set { this.ConditionId = (int)value; }
        }

        /// <summary>
        /// Gets or sets the reminder rule
        /// </summary>
        public int ReminderRuleId { get; set; }

        public CustomerReminderRuleEnum ReminderRule
        {
            get { return (CustomerReminderRuleEnum)ReminderRuleId; }
            set { this.ReminderRuleId = (int)value; }
        }

        /// <summary>
        /// Gets or sets the customer condition
        /// </summary>
        public virtual ICollection<ReminderCondition> Conditions
        {
            get { return _condition ?? (_condition = new List<ReminderCondition>()); }
            protected set { _condition = value; }
        }

        /// <summary>
        /// Gets or sets the reminder level
        /// </summary>
        public virtual ICollection<ReminderLevel> Levels
        {
            get { return _level ?? (_level = new List<ReminderLevel>()); }
            protected set { _level = value; }
        }


        public partial class ReminderCondition : BaseEntity
        {
            private ICollection<ReminderConditionEntity> _entities;
            private ICollection<CustomerRegister> _customerRegister;
            private ICollection<CustomerRegister> _customCustomerAttributes;

            public string Name { get; set; }

            public int ConditionTypeId { get; set; }

            public CustomerReminderConditionTypeEnum ConditionType
            {
                get { return (CustomerReminderConditionTypeEnum)ConditionTypeId; }
                set { this.ConditionTypeId = (int)value; }
            }

            public int ConditionId { get; set; }

            public CustomerReminderConditionEnum Condition
            {
                get { return (CustomerReminderConditionEnum)ConditionId; }
                set { this.ConditionId = (int)value; }
            }

            public virtual ICollection<ReminderConditionEntity> Entity
            {
                get { return _entities ?? (_entities = new List<ReminderConditionEntity>()); }
                protected set { _entities = value; }
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
            
        }

        public partial class ReminderLevel : BaseEntity
        {
            public string Name { get; set; }
            public int Level { get; set; }
            public int Day { get; set; }
            public int Hour { get; set; }
            public int Minutes { get; set; }
            public int EmailAccountId { get; set; }
            public string BccEmailAddresses { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
        }

    }
}
