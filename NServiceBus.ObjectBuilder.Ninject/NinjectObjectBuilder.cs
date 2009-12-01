using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ninject;
using Ninject.Parameters;
using NServiceBus.ObjectBuilder.Common;

namespace NServiceBus.ObjectBuilder.Ninject
{
	public class NinjectObjectBuilder : IContainer
	{
		readonly IKernel _kernel;
		readonly IDictionary<Type, IList<IParameter>> _parameters = new Dictionary<Type, IList<IParameter>>();

		public NinjectObjectBuilder(IKernel kernel)
		{
			_kernel = kernel;
		}

		public object Build(Type typeToBuild)
		{
		  var parameters = _parameters.Where(x => typeToBuild.IsAssignableFrom(x.Key)).SelectMany(x => x.Value).ToArray();
		  var output = _kernel.Get(typeToBuild, parameters);

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
		}

		public void ConfigureProperty(Type component, string property, object value)
		{
			if (!_parameters.ContainsKey(component))
			{
				_parameters[component] = new List<IParameter>();
			}

			_parameters[component].Add(new PropertyValue(property, value));
		}

		public void RegisterSingleton(Type lookupType, object instance)
		{
			_kernel.Bind(lookupType).ToConstant(instance);
		}
	}
}
