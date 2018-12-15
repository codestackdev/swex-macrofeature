//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.sldworks;
using System;
using System.ComponentModel;

namespace CodeStack.SwEx.MacroFeature.Mocks
{
    [Obsolete("Deprecated. Use DimensionEmpty instead")]
    public class DimensionEmpty : Placeholders.DimensionPlaceholder
    {
    }
}
    
namespace CodeStack.SwEx.MacroFeature.Placeholders
{
    /// <summary>
    /// This is a placeholder implementation of SOLIDWORKS dimension
    /// used in <see cref="DisplayDimensionPlaceholder"/>
    /// </summary>
    public class DimensionPlaceholder : Dimension
    {
        internal DimensionPlaceholder()
        {
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public MathVector DimensionLineDirection { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int DrivenState { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public MathVector ExtensionLineDirection { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string FullName { get; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string Name { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ReadOnly { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object ReferencePoints { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double SystemValue { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public DimensionTolerance Tolerance { get; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double Value { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetArcEndCondition(int Index) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Feature GetFeatureOwner() { return null; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string GetNameForSelection() { return ""; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetReferencePointsCount() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetSystemChamferValues(ref double Length, ref double Angle) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double GetSystemValue2(string ConfigName) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object GetSystemValue3(int WhichConfigurations, object Config_names) { return new double[] { 0 }; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string GetToleranceFitValues() { return ""; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object GetToleranceFontInfo() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetToleranceType() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object GetToleranceValues() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetType() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double GetUserValueIn(object Doc) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double GetValue2(string ConfigName) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object GetValue3(int WhichConfigurations, object Config_names) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public MathPoint IGetReferencePoints(int PointsCount) { return null; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double IGetSystemValue3(int WhichConfigurations, int Config_count, ref string Config_names) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double IGetToleranceFontInfo() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double IGetToleranceValues() { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double IGetUserValueIn(ModelDoc Doc) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double IGetUserValueIn2(ModelDoc2 Doc) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double IGetValue3(int WhichConfigurations, int Config_count, ref string Config_names) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsAppliedToAllConfigurations() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsDesignTableDimension() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void ISetReferencePoints(int PointsCount, ref MathPoint RefPoints) { }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int ISetSystemValue3(double NewValue, int WhichConfigurations, int Config_count, ref string Config_names) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void ISetUserValueIn(ModelDoc Doc, double NewValue) { }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int ISetUserValueIn2(ModelDoc Doc, double NewValue, int WhichConfigurations) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int ISetUserValueIn3(ModelDoc2 Doc, double NewValue, int WhichConfigurations) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int ISetValue3(double NewValue, int WhichConfigurations, int Config_count, ref string Config_names) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsReference() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int SetArcEndCondition(int Index, int Condition) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int SetSystemValue2(double NewValue, int WhichConfigurations) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int SetSystemValue3(double NewValue, int WhichConfigurations, object Config_names) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetToleranceFitValues(string NewLValue, string NewUValue) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetToleranceFontInfo(int UseFontScale, double TolScale, double TolHeight) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetToleranceType(int NewType) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool SetToleranceValues(double TolMin, double TolMax) { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public void SetUserValueIn(object Doc, double NewValue) { }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int SetUserValueIn2(object Doc, double NewValue, int WhichConfigurations) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int SetValue2(double NewValue, int WhichConfigurations) { return -1; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int SetValue3(double NewValue, int WhichConfigurations, object Config_names) { return -1; }
    }
}
