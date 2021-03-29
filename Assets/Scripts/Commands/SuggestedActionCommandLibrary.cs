using System.Collections.Generic;
using System.Linq;

namespace Aci.Unity.Commands
{
    public class SuggestedActionCommandLibrary
    {
        private IEnumerable<SuggestedActionCommand> m_Commands;

        public SuggestedActionCommandLibrary(IEnumerable<SuggestedActionCommand> commands)
        {
            m_Commands = commands;
        }

        public ICommand GetCommand(string type)
        {
            return m_Commands.FirstOrDefault(x => x.Type == type);
        }
    }
}
