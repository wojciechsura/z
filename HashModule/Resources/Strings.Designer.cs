﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HashModule.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("HashModule.Resources.Strings", typeof(Strings).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Compute hashes of text.
        /// </summary>
        public static string Hash_ActionComment {
            get {
                return ResourceManager.GetString("Hash_ActionComment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hash.
        /// </summary>
        public static string Hash_ActionDisplay {
            get {
                return ResourceManager.GetString("Hash_ActionDisplay", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Copy MD5 to clipboard.
        /// </summary>
        public static string Hash_Md5_Comment {
            get {
                return ResourceManager.GetString("Hash_Md5_Comment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Choose one of suggestions to copy hash to clipboard!.
        /// </summary>
        public static string Hash_Message_ChooseSuggestion {
            get {
                return ResourceManager.GetString("Hash_Message_ChooseSuggestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hash generator.
        /// </summary>
        public static string Hash_ModuleDisplayName {
            get {
                return ResourceManager.GetString("Hash_ModuleDisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Copy SHA1 to clipboard.
        /// </summary>
        public static string Hash_SHA1_Comment {
            get {
                return ResourceManager.GetString("Hash_SHA1_Comment", resourceCulture);
            }
        }
    }
}
