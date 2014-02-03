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

namespace Convertidor
{

	public partial class MainWindow : Window
	{

		private ImageViewModel viewModel = new ImageViewModel();
		
		public MainWindow()
		{
			InitializeComponent();
			
			this.Loaded += (s, e) => { this.DataContext = this.viewModel; } ;
		}
		
		void Droparea_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			    e.Effects = DragDropEffects.All;
		    else
		    	e.Effects = DragDropEffects.None;
			    	
		}
		
		void Droparea_Drop(object sender, DragEventArgs e)
		{
			this.viewModel.LoadImages((string[]) e.Data.GetData(DataFormats.FileDrop, false));
		}
		
	}
}