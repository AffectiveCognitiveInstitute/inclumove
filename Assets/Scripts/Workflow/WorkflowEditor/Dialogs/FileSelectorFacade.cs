using Aci.Unity.UI.Dialog;
using System.Collections.Generic;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class FileSelectorFacade
    {
        private IDialogService m_DialogService;
        private FileSelectorViewController.Factory m_Factory;

        public FileSelectorFacade(IDialogService dialogService, FileSelectorViewController.Factory factory)
        {
            m_DialogService = dialogService;
            m_Factory = factory;
        }

        /// <summary>
        ///     Opens a file selector dialog.
        /// </summary>
        /// <param name="fileSelected">Callback when file is selected.</param>
        /// <param name="filePaths">File directories to be searched</param>
        /// <param name="filters">Optional filters to be passed, e.g. extensions (*.ext)</param>
        /// <param name="dialogPriority">Dialog Priority. Higher values get displayed first.</param>
        /// <returns></returns>
        public FileSelectorViewController Show(FileSelectedDelegate fileSelected, IEnumerable<string> filePaths, IEnumerable<string> filters, DialogPriority dialogPriority = DialogPriority.High)
        {
            FileSelectorViewController fileSelector = m_Factory.Create(fileSelected, filePaths, filters);
            DialogRequest request = DialogRequest.Create((DialogComponent) fileSelector, dialogPriority);
            m_DialogService.SendRequest(request);
            return fileSelector;
        }
    }
}