﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ScalesAutomation.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.4.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DataImporterEnabled {
            get {
                return ((bool)(this["DataImporterEnabled"]));
            }
            set {
                this["DataImporterEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool SimulationEnabled {
            get {
                return ((bool)(this["SimulationEnabled"]));
            }
            set {
                this["SimulationEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("c:\\Home\\Krs\\Work\\Cantar\\ScalesAutomation\\Distribution\\Logs\\SerialTransmissionSimu" +
            "lation.log")]
        public string SerialTransmissionSimulationPath {
            get {
                return ((string)(this["SerialTransmissionSimulationPath"]));
            }
            set {
                this["SerialTransmissionSimulationPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("COM5")]
        public string ReadCOMPort {
            get {
                return ((string)(this["ReadCOMPort"]));
            }
            set {
                this["ReadCOMPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("COM5")]
        public string WriteCOMPort {
            get {
                return ((string)(this["WriteCOMPort"]));
            }
            set {
                this["WriteCOMPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("CatalogProduseEurocas.xml")]
        public string CatalogFilePath {
            get {
                return ((string)(this["CatalogFilePath"]));
            }
            set {
                this["CatalogFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Logs\\")]
        public string LogFolderPath {
            get {
                return ((string)(this["LogFolderPath"]));
            }
            set {
                this["LogFolderPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("CSVOutput\\")]
        public string CSVOutputPath {
            get {
                return ((string)(this["CSVOutputPath"]));
            }
            set {
                this["CSVOutputPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("CSVBackup\\")]
        public string CSVBackupPath {
            get {
                return ((string)(this["CSVBackupPath"]));
            }
            set {
                this["CSVBackupPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public double MeasurementTollerace {
            get {
                return ((double)(this["MeasurementTollerace"]));
            }
            set {
                this["MeasurementTollerace"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public int ConsecutiveStableMeasurements {
            get {
                return ((int)(this["ConsecutiveStableMeasurements"]));
            }
            set {
                this["ConsecutiveStableMeasurements"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int ZeroThreshold {
            get {
                return ((int)(this["ZeroThreshold"]));
            }
            set {
                this["ZeroThreshold"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server\\Cantariri_Automate\\")]
        public string CSVServerFolderPath {
            get {
                return ((string)(this["CSVServerFolderPath"]));
            }
            set {
                this["CSVServerFolderPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Server\\Cantariri_Manuale\\")]
        public string DataImporterOutputPath {
            get {
                return ((string)(this["DataImporterOutputPath"]));
            }
            set {
                this["DataImporterOutputPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Constalaris")]
        public string ScaleType {
            get {
                return ((string)(this["ScaleType"]));
            }
            set {
                this["ScaleType"] = value;
            }
        }
    }
}
