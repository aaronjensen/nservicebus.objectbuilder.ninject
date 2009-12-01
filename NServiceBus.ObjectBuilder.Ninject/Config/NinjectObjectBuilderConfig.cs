using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Ninject;
using NServiceBus.ObjectBuilder.Common.Config;

namespace NServiceBus.ObjectBuilder.Ninject.Config
{
	public static class NinjectObjectBuilderConfig
	{
		public static Configure NinjectBuilder(this Configure config, IKernel kernel)
		{
		  ConfigureCommon.With(config, new NinjectObjectBuilder(kernel));
			return config;
		}
	}
}
