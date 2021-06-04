using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Syncfusion.UI.Xaml.Diagram;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using System.Windows.Input;

namespace SyncfusionGraphTest
{
    public class MainWindowVM
    {
        public ICommand Fit { get; set; }
        public ICommand Load { get; set; }
        public ICommand Refresh { get; set; }

        public MSAGLLayout Layout { get; set; }
        public DiagramViewModel DiagramModel { get; set; }
        public GeometryGraph MSAGLModel { get; set; }

        public MainWindowVM()
        {
            DiagramModel = new DiagramViewModel()
            {
                Nodes = new NodeCollection(),
                Connectors = new ConnectorCollection(),
                DefaultConnectorType = ConnectorType.PolyLine
            };

            Layout = new MSAGLLayout
            {
                Layout = LayoutType.Sugiyama,
                RoutingSettings = new EdgeRoutingSettings
                {
                    BundlingSettings = new BundlingSettings()
                },
                RoutingMode = EdgeRoutingMode.Spline
            };

            Load = new DelegateCommand(OnLoad);
            Refresh = new DelegateCommand(OnRefresh);
            Fit = new DelegateCommand(OnFit);

            Load.Execute(null);
        }


        private void LoadDiagram(IGraph graph)
        {
            var nodes = graph.Nodes as NodeCollection;
            var connectors = graph.Connectors as ConnectorCollection;
            nodes.Clear();
            connectors.Clear();

            var node1 = new NodeViewModel() { UnitWidth = 50, UnitHeight = 40, Content = "Node1"  };
            nodes.Add(node1);

            var node2 = new NodeViewModel() { UnitWidth = 50, UnitHeight = 40, Content = "Node2" };
            nodes.Add(node2);

            var node3 = new NodeViewModel() { UnitWidth = 50, UnitHeight = 40, Content = "Node3" };
            nodes.Add(node3);

            connectors.Add(new ConnectorViewModel
            {
                SourceNode = node1,
                TargetNode = node2,
                Segments = new ObservableCollection<object> { new StraightSegment() }
            });

            connectors.Add(new ConnectorViewModel
            {
                SourceNode = node1,
                TargetNode = node3,
                Segments = new ObservableCollection<object> { new StraightSegment() }
            });


            MSAGLModel = DiagramModel.ToMSAGL();
        }

        private void OnRefresh(object obj)
        {
            Layout.UpdateLayout(MSAGLModel);
            DiagramModel.UpdateSfDiagram(MSAGLModel);
            Fit.Execute(null);
        }

        private void OnLoad(object obj)
        {
            LoadDiagram(DiagramModel);
            Refresh.Execute(null);
        }

        private void OnFit(object obj)
        {
            if (DiagramModel.Info != null)
            {
                var graph = DiagramModel.Info as IGraphInfo;
                graph.Commands.Zoom.Execute(
                    new ZoomPositionParameter
                    {
                        ZoomTo = 500,
                        ZoomCommand = ZoomCommand.ZoomIn
                    });
                //(graph.ScrollInfo as ScrollViewer).Page.UpdateLayout();
                graph.Commands.FitToPage.Execute(
                    new FitToPageParameter
                    {
                        FitToPage = FitToPage.FitToPage,
                        CanZoomIn = true
                    });
            }
        }
    }
}
