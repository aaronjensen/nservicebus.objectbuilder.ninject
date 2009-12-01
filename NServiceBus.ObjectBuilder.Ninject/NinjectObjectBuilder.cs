using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Ninject;
using Ninject.Parameters;
using Ninject.Planning.Strategies;
using Ninject.Selection;
using Ninject.Selection.Heuristics;
using Ninject.Syntax;
using NServiceBus.ObjectBuilder.Common;

namespace NServiceBus.ObjectBuilder.Ninject
{
	public class NinjectObjectBuilder : IContainer
	{
		readonly IKernel _kernel;
	  readonly ObjectBuilderPropertyHeuristic _heuristic;

		public NinjectObjectBuilder(IKernel kernel)
		{
			_kernel = kernel;

		  _heuristic = new ObjectBuilderPropertyHeuristic();
		  _kernel.Components.Get<ISelector>().InjectionHeuristics.Add(_heuristic);
		}

		public object Build(Type typeToBuild)
		{
		  var output = _kernel.Get(typeToBuild);

      return output;
		}

		public IEnumerable<object> BuildAll(Type typeToBuild)
		{
			return _kernel.GetAll(typeToBuild);
		}

		public void Configure(Type component, ComponentCallModelEnum callModel)
		{
			if (callModel == ComponentCallModelEnum.Singleton)
			{
				_kernel.Bind(component).ToSelf().InSingletonScope();
			} 
			else if (callModel == ComponentCallModelEnum.Singlecall)
			{
				_kernel.Bind(component).ToSelf().InTransientScope();
			}
			else throw new ArgumentException("callModel");

		  _heuristic.RegisteredTypes.Add(component);
		}

		public void ConfigureProperty(Type component, string property, object value)
		{
		  var bindings = _kernel.GetBindings(component);
      if (!bindings.Any()) throw new ArgumentException("Component not registered: " + component);

      foreach (var binding in bindings)
      {
        binding.Parameters.Add(new PropertyValue(property, value));
      }
		}

		public void RegisterSingleton(Type lookupType, object instance)
		{
			_kernel.Bind(lookupType).ToConstant(instance);
		}
	}

  class ObjectBuilderPropertyHeuristic : IInjectionHeuristic
  {
    public IList<Type> RegisteredTypes { get; private set; }

    public ObjectBuilderPropertyHeuristic()
    {
      RegisteredTypes = new List<Type>();
    }

    public void Dispose()
    {
    }

    public INinjectSettings Settings
    {
      get; set;
    }

    public bool ShouldInject(MemberInfo member)
    {
      var propertyInfo = member as PropertyInfo;
      if (propertyInfo == null) return false;

      if (RegisteredTypes.Where(x => propertyInfo.DeclaringType.IsAssignableFrom(x)).Any() && RegisteredTypes.Where(x => propertyInfo.PropertyType.IsAssignableFrom(x)).Any())
        return true;

      return false;
    }
  }
}
