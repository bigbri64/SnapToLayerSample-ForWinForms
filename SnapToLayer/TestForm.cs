﻿using System;
using System.IO;
using System.Windows.Forms;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WinForms;

namespace SnapToLayer
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            winformsMap1.MapUnit = GeographyUnit.DecimalDegree;
            winformsMap1.CurrentExtent = new RectangleShape(-97.7591, 30.3126, -97.7317, 30.2964);
            winformsMap1.BackgroundOverlay.BackgroundBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 198, 255, 255));

            //Displays the World Map Kit as a background.
            ThinkGeo.MapSuite.WinForms.WorldStreetsAndImageryOverlay worldMapKitDesktopOverlay = new ThinkGeo.MapSuite.WinForms.WorldStreetsAndImageryOverlay();
            winformsMap1.Overlays.Add(worldMapKitDesktopOverlay);

            string fileName1 = @"..\..\data\polygon.txt";
            StreamReader sr1 = new StreamReader(fileName1);

            string fileName2 = @"..\..\data\line.txt";
            StreamReader sr2 = new StreamReader(fileName2);

            //SnapToLayerEditInteractiveOverlay to snap dragged control point to nearest vertex of layer if within tolerance.
            SnapToLayerEditInteractiveOverlay snapToLayerEditInteractiveOverlay = new SnapToLayerEditInteractiveOverlay();

            //inMemoryFeatureLayer used to be snapped to.
            InMemoryFeatureLayer polygonInMemoryFeatureLayer = new InMemoryFeatureLayer();
            polygonInMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.Park1;
            polygonInMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            polygonInMemoryFeatureLayer.InternalFeatures.Add("Polygon", new Feature(BaseShape.CreateShapeFromWellKnownData(sr1.ReadLine())));

            LayerOverlay inMemoryOverlay = new LayerOverlay();
            inMemoryOverlay.Layers.Add("InMemoryFeatureLayer", polygonInMemoryFeatureLayer);
            winformsMap1.Overlays.Add("InMemoryOverlay", inMemoryOverlay);

            snapToLayerEditInteractiveOverlay.EditShapesLayer.InternalFeatures.Add("MultiLine", new Feature(BaseShape.CreateShapeFromWellKnownData(sr2.ReadLine())));

            //Sets the PointStyle for the non dragged control points.
            snapToLayerEditInteractiveOverlay.ControlPointStyle = new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.StandardColors.PaleGoldenrod), new GeoPen(GeoColor.StandardColors.Black), 8);
            //Sets the PointStyle for the dragged control points.
            snapToLayerEditInteractiveOverlay.DraggedControlPointStyle = new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.StandardColors.Red), new GeoPen(GeoColor.StandardColors.Orange, 2), 10);

            snapToLayerEditInteractiveOverlay.ToSnapInMemoryFeatureLayer = polygonInMemoryFeatureLayer;

            //Example using Screen (Pixel) coordinates for tolerance.
            snapToLayerEditInteractiveOverlay.ToleranceType = ToleranceCoordinates.Screen;
            snapToLayerEditInteractiveOverlay.Tolerance = 25;

            //Example using World coordinates for tolerance.
            //snapToLayerEditInteractiveOverlay.ToleranceType = ToleranceCoordinates.World;
            //snapToLayerEditInteractiveOverlay.Tolerance = 150;
            //snapToLayerEditInteractiveOverlay.ToleranceUnit = DistanceUnit.Meter;

            snapToLayerEditInteractiveOverlay.CalculateAllControlPoints();

            winformsMap1.EditOverlay = snapToLayerEditInteractiveOverlay;

            winformsMap1.Refresh();
        }

        private void winformsMap1_MouseMove(object sender, MouseEventArgs e)
        {
            //Displays the X and Y in screen coordinates.
            statusStrip1.Items["toolStripStatusLabelScreen"].Text = "X:" + e.X + " Y:" + e.Y;

            //Gets the PointShape in world coordinates from screen coordinates.
            PointShape pointShape = ExtentHelper.ToWorldCoordinate(winformsMap1.CurrentExtent, new ScreenPointF(e.X, e.Y), winformsMap1.Width, winformsMap1.Height);

            //Displays world coordinates.
            statusStrip1.Items["toolStripStatusLabelWorld"].Text = "(world) X:" + Math.Round(pointShape.X, 4) + " Y:" + Math.Round(pointShape.Y, 4);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
