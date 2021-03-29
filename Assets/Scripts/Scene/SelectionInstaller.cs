using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace Aci.Unity.Scene
{
    class SelectionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ISelectionService>().To<SelectionService>().AsSingle();
        }
    }
}
