using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using Task = System.Threading.Tasks.Task;

namespace Böxtensions
{
    public sealed class ToggleDollarCommand
    {
        void Execute(object sender, EventArgs e)
        {
            var service = ServiceProvider.GetServiceAsync(typeof(SVsTextManager)).Result;
            var textManager = service as IVsTextManager2;
            IVsTextView view;
            int result = textManager.GetActiveView2(1, null, (uint)_VIEWFRAMETYPE.vftCodeWindow, out view);

            if (view == null)
                return;

            view.GetSelection(out var anchor_line, out var anchor_col, out var end_line, out var end_col);

            if (anchor_col != end_col || anchor_line != end_line )
                return;

            view.SetSelection(anchor_line, 0, anchor_line, end_col);
            view.GetSelectedText(out var line);

            var action = ToggleDollar(line, end_col);
            if (action.do_nothing)
                return;

            view.ReplaceTextOnLine(end_line, action.replace_start, action.replace_length,
                action.substitution, action.substitution.Length);

            var cursor = end_col + action.cursor_offset;
            view.SetSelection(end_line, cursor, end_line, cursor);
        }

        public struct ReplaceAction
        {
            public bool do_nothing;
            public int replace_start;
            public int replace_length;
            public string substitution;
            public int cursor_offset;
        }

        public static ReplaceAction ToggleDollar(string line, int cursor)
        {
            var fail = new ReplaceAction { do_nothing = true };
            var max_index = Math.Min(line.Length - 1, cursor);
            var index_of_left_quote = line.LastIndexOf('"', max_index);

            if (index_of_left_quote < 0)
                return fail;

            var index_of_dollar =
                index_of_left_quote > 0 && line[index_of_left_quote - 1] == '@'
                ? index_of_left_quote - 1
                : index_of_left_quote;

            if (index_of_dollar == 0 || line[index_of_dollar-1] != '$') {
                return new ReplaceAction {
                    replace_start = index_of_dollar,
                    replace_length = 0,
                    substitution = "$",
                    cursor_offset = +1
                };
            } else {
                return new ReplaceAction {
                    replace_start = index_of_dollar - 1,
                    replace_length = 1,
                    substitution = "",
                    cursor_offset = -1
                };
            }
        }
    
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("1514e479-da39-42f8-aec9-f629abf847cb");

        readonly AsyncPackage package;

        ToggleDollarCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static ToggleDollarCommand Instance { get; private set; }

        Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider { get { return this.package; } }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new ToggleDollarCommand(package, commandService);
        }
    }
}
