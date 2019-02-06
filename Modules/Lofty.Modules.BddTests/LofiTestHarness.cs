using System;
using Microsoft.Extensions.DependencyInjection;

namespace Lofty.Modules.BddTests
{
  public class LofiTestHarness : IDisposable
  {
    public LofiTestHarness()
    {
      var services = new ServiceCollection();
    }

    public void Dispose()
    {
    }
  }
}