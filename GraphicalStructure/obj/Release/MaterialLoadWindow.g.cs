﻿#pragma checksum "..\..\MaterialLoadWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6DDE12ED88C902404027437328689CD2"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using GraphicalStructure;
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


namespace GraphicalStructure {
    
    
    /// <summary>
    /// MaterialLoadWindow
    /// </summary>
    public partial class MaterialLoadWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\MaterialLoadWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBox_materialName;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\MaterialLoadWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBox_matName;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\MaterialLoadWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBox_soeName;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\MaterialLoadWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Expander expander_materialName;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\MaterialLoadWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Expander expander_matName;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\MaterialLoadWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Expander expander_soeName;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\MaterialLoadWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_curd;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\MaterialLoadWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_Cancel;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\MaterialLoadWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_Cancel_Copy;
        
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
            System.Uri resourceLocater = new System.Uri("/GraphicalStructure;component/materialloadwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MaterialLoadWindow.xaml"
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
            this.listBox_materialName = ((System.Windows.Controls.ListBox)(target));
            
            #line 11 "..\..\MaterialLoadWindow.xaml"
            this.listBox_materialName.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.selectionHandle);
            
            #line default
            #line hidden
            return;
            case 2:
            this.listBox_matName = ((System.Windows.Controls.ListBox)(target));
            
            #line 14 "..\..\MaterialLoadWindow.xaml"
            this.listBox_matName.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.selectionHandle);
            
            #line default
            #line hidden
            return;
            case 3:
            this.listBox_soeName = ((System.Windows.Controls.ListBox)(target));
            
            #line 18 "..\..\MaterialLoadWindow.xaml"
            this.listBox_soeName.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.selectionHandle);
            
            #line default
            #line hidden
            return;
            case 4:
            this.expander_materialName = ((System.Windows.Controls.Expander)(target));
            
            #line 22 "..\..\MaterialLoadWindow.xaml"
            this.expander_materialName.Expanded += new System.Windows.RoutedEventHandler(this.sortedByMaterialName);
            
            #line default
            #line hidden
            
            #line 22 "..\..\MaterialLoadWindow.xaml"
            this.expander_materialName.Collapsed += new System.Windows.RoutedEventHandler(this.sortedByMaterialName);
            
            #line default
            #line hidden
            return;
            case 5:
            this.expander_matName = ((System.Windows.Controls.Expander)(target));
            
            #line 24 "..\..\MaterialLoadWindow.xaml"
            this.expander_matName.Expanded += new System.Windows.RoutedEventHandler(this.sortedByMatName);
            
            #line default
            #line hidden
            
            #line 24 "..\..\MaterialLoadWindow.xaml"
            this.expander_matName.Collapsed += new System.Windows.RoutedEventHandler(this.sortedByMatName);
            
            #line default
            #line hidden
            return;
            case 6:
            this.expander_soeName = ((System.Windows.Controls.Expander)(target));
            
            #line 26 "..\..\MaterialLoadWindow.xaml"
            this.expander_soeName.Expanded += new System.Windows.RoutedEventHandler(this.sortedBySoeName);
            
            #line default
            #line hidden
            
            #line 26 "..\..\MaterialLoadWindow.xaml"
            this.expander_soeName.Collapsed += new System.Windows.RoutedEventHandler(this.sortedBySoeName);
            
            #line default
            #line hidden
            return;
            case 7:
            this.button_curd = ((System.Windows.Controls.Button)(target));
            
            #line 28 "..\..\MaterialLoadWindow.xaml"
            this.button_curd.Click += new System.Windows.RoutedEventHandler(this.confirmButtonClick);
            
            #line default
            #line hidden
            return;
            case 8:
            this.button_Cancel = ((System.Windows.Controls.Button)(target));
            
            #line 29 "..\..\MaterialLoadWindow.xaml"
            this.button_Cancel.Click += new System.Windows.RoutedEventHandler(this.cancelButtonClick);
            
            #line default
            #line hidden
            return;
            case 9:
            this.button_Cancel_Copy = ((System.Windows.Controls.Button)(target));
            
            #line 30 "..\..\MaterialLoadWindow.xaml"
            this.button_Cancel_Copy.Click += new System.Windows.RoutedEventHandler(this.deleteMaterialInDatabase);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

