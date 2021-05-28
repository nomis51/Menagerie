using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Menagerie.Models.Abstractions;

namespace Menagerie.Models
{
    public class ReactiveProperty<T> : IReactiveProperty
    {
        private T _value;

        public T Value
        {
            get => CustomGet != null ? CustomGet(_value) : _value;
            set
            {
                _value = CustomSet != null ? CustomSet(value) : value;
                Notify();
            }
        }

        public string Name { get; private set; }
        private readonly Screen _context;

        public List<IReactiveProperty> AdditionalReactivePropertiesToNotify { get; set; }
        public List<string> AdditionalPropertiesToNotify { get; set; }
        public Func<T, T> CustomGet { get; set; }
        public Func<T, T> CustomSet { get; set; }

        public ReactiveProperty(string name, Screen context, T defaultValue = default)
        {
            _value = defaultValue;
            _context = context;
            Name = name;
            AdditionalReactivePropertiesToNotify = new List<IReactiveProperty>();
            AdditionalPropertiesToNotify = new List<string>();
        }

        public void Notify()
        {
            _context.NotifyOfPropertyChange(Name);
            AdditionalReactivePropertiesToNotify.ForEach(prop => prop.Notify());
            AdditionalPropertiesToNotify.ForEach(prop => _context.NotifyOfPropertyChange(prop));
        }
    }
}