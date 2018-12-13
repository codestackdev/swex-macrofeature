//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.sldworks;

namespace CodeStack.SwEx.MacroFeature.Mocks
{
    public class DimensionEmpty : Dimension
    {
        public MathVector DimensionLineDirection { get; set; }
        public int DrivenState { get; set; }
        public MathVector ExtensionLineDirection { get; set; }
        public string FullName { get; }
        public string Name { get; set; }
        public bool ReadOnly { get; set; }
        public object ReferencePoints { get; set; }
        public double SystemValue { get; set; }
        public DimensionTolerance Tolerance { get; }
        public double Value { get; set; }

        public int GetArcEndCondition(int Index) { return -1; }
        public Feature GetFeatureOwner() { return null; }
        public string GetNameForSelection() { return ""; }
        public int GetReferencePointsCount() { return -1; }
        public bool GetSystemChamferValues(ref double Length, ref double Angle) { return false; }
        public double GetSystemValue2(string ConfigName) { return -1; }
        public object GetSystemValue3(int WhichConfigurations, object Config_names) { return new double[] { 0 }; }
        public string GetToleranceFitValues() { return ""; }
        public object GetToleranceFontInfo() { return -1; }
        public int GetToleranceType() { return -1; }
        public object GetToleranceValues() { return -1; }
        public int GetType() { return -1; }
        public double GetUserValueIn(object Doc) { return -1; }
        public double GetValue2(string ConfigName) { return -1; }
        public object GetValue3(int WhichConfigurations, object Config_names) { return -1; }
        public MathPoint IGetReferencePoints(int PointsCount) { return null; }
        public double IGetSystemValue3(int WhichConfigurations, int Config_count, ref string Config_names) { return -1; }
        public double IGetToleranceFontInfo() { return -1; }
        public double IGetToleranceValues() { return -1; }
        public double IGetUserValueIn(ModelDoc Doc) { return -1; }
        public double IGetUserValueIn2(ModelDoc2 Doc) { return -1; }
        public double IGetValue3(int WhichConfigurations, int Config_count, ref string Config_names) { return -1; }
        public bool IsAppliedToAllConfigurations() { return false; }
        public bool IsDesignTableDimension() { return false; }
        public void ISetReferencePoints(int PointsCount, ref MathPoint RefPoints) { }
        public int ISetSystemValue3(double NewValue, int WhichConfigurations, int Config_count, ref string Config_names) { return -1; }
        public void ISetUserValueIn(ModelDoc Doc, double NewValue) { }
        public int ISetUserValueIn2(ModelDoc Doc, double NewValue, int WhichConfigurations) { return -1; }
        public int ISetUserValueIn3(ModelDoc2 Doc, double NewValue, int WhichConfigurations) { return -1; }
        public int ISetValue3(double NewValue, int WhichConfigurations, int Config_count, ref string Config_names) { return -1; }
        public bool IsReference() { return false; }
        public int SetArcEndCondition(int Index, int Condition) { return -1; }
        public int SetSystemValue2(double NewValue, int WhichConfigurations) { return -1; }
        public int SetSystemValue3(double NewValue, int WhichConfigurations, object Config_names) { return -1; }
        public bool SetToleranceFitValues(string NewLValue, string NewUValue) { return false; }
        public bool SetToleranceFontInfo(int UseFontScale, double TolScale, double TolHeight) { return false; }
        public bool SetToleranceType(int NewType) { return false; }
        public bool SetToleranceValues(double TolMin, double TolMax) { return false; }
        public void SetUserValueIn(object Doc, double NewValue) { }
        public int SetUserValueIn2(object Doc, double NewValue, int WhichConfigurations) { return -1; }
        public int SetValue2(double NewValue, int WhichConfigurations) { return -1; }
        public int SetValue3(double NewValue, int WhichConfigurations, object Config_names) { return -1; }
    }
}
