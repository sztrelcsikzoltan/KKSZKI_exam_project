﻿#pragma checksum "..\..\DataBinding_ObservableCollection_Users.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "5F0323CAFBFEF6A7350D3BE89D939BB476ADE9017DAD53FAFF10B05FD4D432EF"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
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
using WPF.NET_Templates.Templates;


namespace WPF.NET_Templates.Templates {
    
    
    /// <summary>
    /// DataBinding_ObservableCollection_Users
    /// </summary>
    public partial class DataBinding_ObservableCollection_Users : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 53 "..\..\DataBinding_ObservableCollection_Users.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView listview;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\DataBinding_ObservableCollection_Users.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GridView gridview;
        
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
            System.Uri resourceLocater = new System.Uri("/WPF.NET_Templates;component/databinding_observablecollection_users.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\DataBinding_ObservableCollection_Users.xaml"
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
            this.listview = ((System.Windows.Controls.ListView)(target));
            
            #line 53 "..\..\DataBinding_ObservableCollection_Users.xaml"
            this.listview.Loaded += new System.Windows.RoutedEventHandler(this.listview_Loaded);
            
            #line default
            #line hidden
            
            #line 53 "..\..\DataBinding_ObservableCollection_Users.xaml"
            this.listview.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.listview_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.gridview = ((System.Windows.Controls.GridView)(target));
            return;
            case 3:
            
            #line 68 "..\..\DataBinding_ObservableCollection_Users.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AddNumber);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 69 "..\..\DataBinding_ObservableCollection_Users.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.DeleteNumber);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
