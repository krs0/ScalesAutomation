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
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.5.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
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
        [global::System.Configuration.DefaultSettingValueAttribute("COM6")]
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
        [global::System.Configuration.DefaultSettingValueAttribute("CatalogProduse.xml")]
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
        [global::System.Configuration.DefaultSettingValueAttribute("CSVServer\\")]
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
        [global::System.Configuration.DefaultSettingValueAttribute("SerialTransmissionSimulation.txt")]
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
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SimulationEnabled {
            get {
                return ((bool)(this["SimulationEnabled"]));
            }
            set {
                this["SimulationEnabled"] = value;
            }
        }
    }
}
