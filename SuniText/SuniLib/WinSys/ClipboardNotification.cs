using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuniText.SuniLib.WinSys
{
    public sealed class ClipboardNotification
    {
        public const int WM_CLIPBOARDUPDATE = 0x031D;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool AddClipboardFormatListener(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

        /// <summary>
        /// Occurs when the contents of the clipboard is updated.
        /// </summary>
        public event MethodInvoker ClipboardUpdate;

        public ClipboardNotification()
        {
            new NotificationForm(OnClipboardUpdate);
        }

        /// <summary>
        /// Raises the <see cref="ClipboardUpdate"/> event.
        /// </summary>
        /// <param name="e">Event arguments for the event.</param>
        private void OnClipboardUpdate()
        {
            var handler = ClipboardUpdate;
            if (handler != null)
                handler();
        }

        /// <summary>
        /// Hidden form to recieve the WM_CLIPBOARDUPDATE message.
        /// </summary>
        private class NotificationForm : Form
        {
            private MethodInvoker onClipboardUpdate;
            private IntPtr hWnd;
            private int lastTickCount;

            public NotificationForm(MethodInvoker onClipboardUpdate)
            {
                this.onClipboardUpdate = onClipboardUpdate;
                this.hWnd = this.Handle;
                AddClipboardFormatListener(this.hWnd);
            }

            ~NotificationForm()
            {
                RemoveClipboardFormatListener(this.hWnd);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_CLIPBOARDUPDATE && this.onClipboardUpdate != null)
                {
                    //列表类解析，会阻止时间，用此法不行，再用lastText方法
                    if (Environment.TickCount - this.lastTickCount >= 200)
                        this.onClipboardUpdate();
                    this.lastTickCount = Environment.TickCount;
                    m.Result = IntPtr.Zero;
                }
                base.WndProc(ref m);
            }
        }
    }
}
