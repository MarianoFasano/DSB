﻿#pragma checksum "..\..\..\user_controls\ExpItem.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "3C372F9123B577FD0BC001F1CA45E920C6C3ED237A996D826B62A06C4F0F8F61"
//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.42000
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

using DataSetBuilder.user_controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace DataSetBuilder.user_controls {
    
    
    /// <summary>
    /// ExpItem
    /// </summary>
    public partial class ExpItem : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\user_controls\ExpItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid ExpGrid;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\user_controls\ExpItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel DepoDockPanel;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\user_controls\ExpItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Menu Modifica;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\user_controls\ExpItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem ViewCommentMenu;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\user_controls\ExpItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem DepoDirectoryMenu;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\user_controls\ExpItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem DeleteDepoMenu;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\user_controls\ExpItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox DepoComment;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\user_controls\ExpItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox DepositionViewer;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DataSetBuilder;component/user_controls/expitem.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\user_controls\ExpItem.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.ExpGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.DepoDockPanel = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 3:
            this.Modifica = ((System.Windows.Controls.Menu)(target));
            return;
            case 4:
            this.ViewCommentMenu = ((System.Windows.Controls.MenuItem)(target));
            
            #line 23 "..\..\..\user_controls\ExpItem.xaml"
            this.ViewCommentMenu.Click += new System.Windows.RoutedEventHandler(this.ViewCommentMenu_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.DepoDirectoryMenu = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 6:
            this.DeleteDepoMenu = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 7:
            this.DepoComment = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.DepositionViewer = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

