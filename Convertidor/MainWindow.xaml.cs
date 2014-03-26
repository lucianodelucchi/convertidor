// -----------------------------------------------------------------------------
//  <copyright file="MainWindow.xaml.cs" company="">
//      Copyright (c) 
//  </copyright>
// -----------------------------------------------------------------------------
namespace Convertidor
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    
    using Convertidor.ViewModels;
    
    /// <summary>
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        private ImageViewModel viewModel = new ImageViewModel();
        
        /// <summary>
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            
            this.Loaded += (s, e) => { this.DataContext = this.viewModel; };
        }
        
        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Droparea_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        
        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Droparea_Drop(object sender, DragEventArgs e)
        {
            this.viewModel.LoadImages((string[])e.Data.GetData(DataFormats.FileDrop, false));
        }
    }
}