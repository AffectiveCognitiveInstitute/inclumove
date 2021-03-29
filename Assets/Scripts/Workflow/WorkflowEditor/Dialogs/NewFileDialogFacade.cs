using Aci.Unity.UI.Dialog;
using System.Collections.Generic;

namespace Aci.Unity.Workflow.WorkflowEditor
{
    public class NewFileDialogFacade
    {
        private IDialogService m_DialogService;
        private NewFileDialogViewController.Factory m_Factory;

        public NewFileDialogFacade(IDialogService dialogService, NewFileDialogViewController.Factory factory)
        {
            m_DialogService = dialogService;
            m_Factory = factory;
        }

        /// <summary>
        ///     Opens a new file dialog.
        /// </summary>
        /// <param name="fileSelected">Callback when file is selected.</param>
        /// <param name="targetPath">Target path at which to create the file.</param>
        /// <param name="fileType">Target filetype extension (*.ext).</param>
        /// <param name="dialogPriority">Dialog Priority. Higher values get displayed first.</param>
        /// <returns></returns>
        public NewFileDialogViewController Show(FileSelectedDelegate fileSelected, string targetPath, string fileType, DialogPriority dialogPriority = DialogPriority.High)
        {
            NewFileDialogViewController newFileDialog = m_Factory.Create(fileSelected, targetPath, fileType);
            DialogRequest request = DialogRequest.Create((DialogComponent)newFileDialog, dialogPriority);
            m_DialogService.SendRequest(request);
            return newFileDialog;
        }
    }
}