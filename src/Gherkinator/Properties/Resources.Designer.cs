﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gherkinator.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Gherkinator.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No action was defined for step &apos;{0}&apos; in scenario &apos;{1}&apos; from feature &apos;{2}&apos;..
        /// </summary>
        internal static string MissingAction {
            get {
                return ResourceManager.GetString("MissingAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No action was defined for step &apos;{0}&apos; in scenario &apos;{1}&apos; at &apos;{2}({3},{4})&apos;.
        /// </summary>
        internal static string MissingActionInFile {
            get {
                return ResourceManager.GetString("MissingActionInFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Scenario &apos;{0}&apos; not found in feature document..
        /// </summary>
        internal static string ScenarioNotFound {
            get {
                return ResourceManager.GetString("ScenarioNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Scenario &apos;{0}&apos; not found in feature file &apos;{1}&apos;..
        /// </summary>
        internal static string ScenarioNotFoundInFile {
            get {
                return ResourceManager.GetString("ScenarioNotFoundInFile", resourceCulture);
            }
        }
    }
}
