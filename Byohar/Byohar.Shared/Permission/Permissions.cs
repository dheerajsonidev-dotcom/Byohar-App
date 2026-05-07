using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Byohar.Shared.Permission
{
    public static class Permissions
    {
        [DisplayName("Users")]
        [Description("Users Permissions")]
        public static class Users
        {

            [Display(Name = "All")]
            public const string All = "Permissions.Users.All"; 
            [Display(Name = "User View")]
            public const string View = "Permissions.Users.View";
            [Display(Name = "Create User")]
            public const string Create = "Permissions.Users.Create";
            [Display(Name = "Edit User")]
            public const string Edit = "Permissions.Users.Edit";
            [Display(Name = "Delete User")]
            public const string Delete = "Permissions.Users.Delete";
            [Display(Name = "Export User")]
            public const string Export = "Permissions.Users.Export";
        }

        [DisplayName("Roles")]
        [Description("Roles Permissions")]
        public static class Roles
        {
            [Display(Name = "All")]
            public const string All = "Permissions.Roles.All";
            [Display(Name = "Role View")]
            public const string View = "Permissions.Roles.View";
            [Display(Name = "Create Role")]
            public const string Create = "Permissions.Roles.Create";
            [Display(Name = "Edit Role")]
            public const string Edit = "Permissions.Roles.Edit";
            [Display(Name = "Delete Role")]
            public const string Delete = "Permissions.Roles.Delete";
        }

        [DisplayName("Role Permissions")]
        [Description("Role Claims Permissions")]
        public static class RolePermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.RolePermissions.All";
            [Display(Name = "Role Permission View")]
            public const string View = "Permissions.RolePermissions.View";
            [Display(Name = "Create Role Permission")]
            public const string Create = "Permissions.RolePermissions.Create";
            [Display(Name = "Edit Role Permission")]
            public const string Edit = "Permissions.RolePermissions.Edit";
            [Display(Name = "Delete Role Permission")]
            public const string Delete = "Permissions.RolePermissions.Delete";

        }

        [DisplayName("Team Permissions")]
        [Description("Team Permissions")]
        public static class TeamPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.TeamPermissions.All";
            [Display(Name = "Team View")]
            public const string View = "Permissions.TeamPermissions.View";
            [Display(Name = "Create Team Permission")]
            public const string Create = "Permissions.TeamPermissions.Create";
            [Display(Name = "Edit Team Permission")]
            public const string Edit = "Permissions.TeamPermissions.Edit";
            [Display(Name = "Delete Team Permission")]
            public const string Delete = "Permissions.TeamPermissions.Delete";
            [Display(Name = "Export Team")]
            public const string Export = "Permissions.TeamPermissions.Export";
        }

        [DisplayName("Asset")]
        [Description("Asset Permissions")]
        public static class AssetPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.AssetPermissions.All";
            [Display(Name = "Asset View")]
            public const string View = "Permissions.AssetPermissions.View";
            [Display(Name = "Create Asset")]
            public const string Create = "Permissions.AssetPermissions.Create";
            [Display(Name = "Edit Asset")]
            public const string Edit = "Permissions.AssetPermissions.Edit";
            [Display(Name = "Delete Asset")]
            public const string Delete = "Permissions.AssetPermissions.Delete";
            [Display(Name = "Export Asset")]
            public const string Export = "Permissions.AssetPermissions.Export"; 
            [Display(Name = "Archive Asset")]
            public const string Archive = "Permissions.AssetPermissions.Archive";
            [Display(Name = "View Archive Asset")]
            public const string ViewArchive = "Permissions.AssetPermissions.ViewArchive";
            [Display(Name = "Edit Archive Asset")]
            public const string EditArchive = "Permissions.AssetPermissions.EditArchive";
        }
        
        [DisplayName("Building")]
        [Description("Building Permissions")]
        public static class BuildingPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.BuildingPermissions.All";
            [Display(Name = "Building View")]
            public const string View = "Permissions.BuildingPermissions.View";
            [Display(Name = "Create Building")]
            public const string Create = "Permissions.BuildingPermissions.Create";
            [Display(Name = "Edit Building")]
            public const string Edit = "Permissions.BuildingPermissions.Edit";
            [Display(Name = "Delete Building")]
            public const string Delete = "Permissions.BuildingPermissions.Delete";
            [Display(Name = "Export Building")]
            public const string Export = "Permissions.BuildingPermissions.Export";           
            [Display(Name = "Add Location Floor")]
            public const string AddLocationFloor = "Permissions.BuildingPermissions.AddLocationFloor";
        }


        [DisplayName("Site")]
        [Description("Site Permissions")]
        public static class SitePermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.SitePermissions.All";
            [Display(Name = "Site View")]
            public const string View = "Permissions.SitePermissions.View";
            [Display(Name = "Create Site")]
            public const string Create = "Permissions.SitePermissions.Create";
            [Display(Name = "Edit Site")]
            public const string Edit = "Permissions.SitePermissions.Edit";
            [Display(Name = "Delete Site")]
            public const string Delete = "Permissions.SitePermissions.Delete";
            [Display(Name = "Export Site")]
            public const string Export = "Permissions.SitePermissions.Export";

        }

        [DisplayName("Supplier")]
        [Description("Supplier Permissions")]
        public static class SupplierPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.SupplierPermissions.All";
            [Display(Name = "Supplier View")]
            public const string View = "Permissions.SupplierPermissions.View";
            [Display(Name = "Create Supplier")]
            public const string Create = "Permissions.SupplierPermissions.Create";
            [Display(Name = "Edit Supplier")]
            public const string Edit = "Permissions.SupplierPermissions.Edit";
            [Display(Name = "Delete Supplier")]
            public const string Delete = "Permissions.SupplierPermissions.Delete";
            [Display(Name = "Export Supplier")]
            public const string Export = "Permissions.SupplierPermissions.Export";
        }
        
        [DisplayName("Technician")]
        [Description("Technician Permissions")]
        public static class TechnicianPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.TechnicianPermissions.All";
            [Display(Name = "Technician View")]
            public const string View = "Permissions.TechnicianPermissions.View";
            [Display(Name = "Create Technician")]
            public const string Create = "Permissions.TechnicianPermissions.Create";
            [Display(Name = "Edit Technician")]
            public const string Edit = "Permissions.TechnicianPermissions.Edit";
            [Display(Name = "Delete Technician")]
            public const string Delete = "Permissions.TechnicianPermissions.Delete";
            [Display(Name = "Export Technician")]
            public const string Export = "Permissions.TechnicianPermissions.Export";
        }

        [DisplayName("Contract")]
        [Description("Contract Permissions")]
        public static class ContractPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.ContractPermissions.All";
            [Display(Name = "Contract View")]
            public const string View = "Permissions.ContractPermissions.View";
            [Display(Name = "Create Contract")]
            public const string Create = "Permissions.ContractPermissions.Create";
            [Display(Name = "Edit Contract")]
            public const string Edit = "Permissions.ContractPermissions.Edit";
            [Display(Name = "Delete Contract")]
            public const string Delete = "Permissions.ContractPermissions.Delete";
        }

        [DisplayName("Contact")]
        [Description("Contact Permissions")]
        public static class ContactPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.ContactPermissions.All";
            [Display(Name = "Contact View")]
            public const string View = "Permissions.ContactPermissions.View";
            [Display(Name = "Create Contact")]
            public const string Create = "Permissions.ContactPermissions.Create";
            [Display(Name = "Edit Contact")]
            public const string Edit = "Permissions.ContactPermissions.Edit";
            [Display(Name = "Delete Contact")]
            public const string Delete = "Permissions.ContactPermissions.Delete";
            [Display(Name = "Export Contact")]
            public const string Export = "Permissions.ContactPermissions.Export";
        }

        [DisplayName("Work Order")]
        [Description("Work Order Permissions")]
        public static class WorkOrderPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.WorkOrderPermissions.All";
            [Display(Name = "Work Order View")]
            public const string View = "Permissions.WorkOrderPermissions.View";
            [Display(Name = "Create Work Order")]
            public const string Create = "Permissions.WorkOrderPermissions.Create";
            [Display(Name = "Edit Work Order")]
            public const string Edit = "Permissions.WorkOrderPermissions.Edit";
            [Display(Name = "Delete Work Order")]
            public const string Delete = "Permissions.WorkOrderPermissions.Delete";
            [Display(Name = "Export Work Order")]
            public const string Export = "Permissions.WorkOrderPermissions.Export";
            [Display(Name = "Archive Work Order")]
            public const string Archive = "Permissions.WorkOrderPermissions.Archive"; 
            [Display(Name = "View Archive Work Order")]
            public const string ViewArchive = "Permissions.WorkOrderPermissions.ViewArchive";
            [Display(Name = "Edit Archive Work Order")]
            public const string EditArchive = "Permissions.WorkOrderPermissions.EditArchive";
        }

        [DisplayName("PPM")]
        [Description("PPM Permissions")]
        public static class PPMPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.PPMPermissions.All";
            [Display(Name = "PPM View")]
            public const string View = "Permissions.PPMPermissions.View";
            [Display(Name = "Create PPM")]
            public const string Create = "Permissions.PPMPermissions.Create";
            [Display(Name = "Edit PPM")]
            public const string Edit = "Permissions.PPMPermissions.Edit";
            [Display(Name = "Delete PPM")]
            public const string Delete = "Permissions.PPMPermissions.Delete";
            [Display(Name = "Export PPM")]
            public const string Export = "Permissions.PPMPermissions.Export";
            [Display(Name = "Archive PPM")]
            public const string Archive = "Permissions.PPMPermissions.Archive";
            [Display(Name = "View Archive PPM")]
            public const string ViewArchive = "Permissions.PPMPermissions.ViewArchive";
            [Display(Name = "Edit Archive PPM")]
            public const string EditArchive = "Permissions.PPMPermissions.EditArchive";
        }

        [DisplayName("Documents")]
        [Description("Documents Permissions")]
        public static class DocumentsPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.DocumentsPermissions.All";
            [Display(Name = "Documents View")]
            public const string View = "Permissions.DocumentsPermissions.View";
            [Display(Name = "Create Documents")]
            public const string Create = "Permissions.DocumentsPermissions.Create";
            [Display(Name = "Edit Documents")]
            public const string Edit = "Permissions.DocumentsPermissions.Edit";
            [Display(Name = "Delete Documents")]
            public const string Delete = "Permissions.DocumentsPermissions.Delete";         
        }   
        
        [DisplayName("Stock")]
        [Description("Stock Permissions")]
        public static class StockPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.StockPermissions.All";
            [Display(Name = "Stocks View")]
            public const string View = "Permissions.StockPermissions.View";
            [Display(Name = "Create Stock")]
            public const string Create = "Permissions.StockPermissions.Create";
            [Display(Name = "Export Stock")]
            public const string Export = "Permissions.StockPermissions.Export";
            [Display(Name = "Edit Stock")]
            public const string Edit = "Permissions.StockPermissions.Edit";
            [Display(Name = "Delete Stock")]
            public const string Delete = "Permissions.StockPermissions.Delete";         
        }

        [DisplayName("Purchase Order")]
        [Description("Purchase Order Permissions")]
        public static class PurchaseOrderPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.PurchaseOrderPermissions.All";
            [Display(Name = "Purchase Order View")]
            public const string View = "Permissions.PurchaseOrderPermissions.View";
            [Display(Name = "Create Purchase Order")]
            public const string Create = "Permissions.PurchaseOrderPermissions.Create";
            [Display(Name = "Edit Purchase Order")]
            public const string Edit = "Permissions.PurchaseOrderPermissions.Edit";
            [Display(Name = "Export Purchase Order")]
            public const string Export = "Permissions.PurchaseOrderPermissions.Export";
            [Display(Name = "Delete Purchase Order")]
            public const string Delete = "Permissions.PurchaseOrderPermissions.Delete";
        }

        [DisplayName("Quotes")]
        [Description("Quotes Permissions")]
        public static class QuotesPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.QuotesPermissions.All";
            [Display(Name = "Quote View")]
            public const string View = "Permissions.QuotesPermissions.View";
            [Display(Name = "Create Quote")]
            public const string Create = "Permissions.QuotesPermissions.Create";
            [Display(Name = "Edit Quote")]
            public const string Edit = "Permissions.QuotesPermissions.Edit";
            [Display(Name = "Export Quote")]
            public const string Export = "Permissions.QuotesPermissions.Export";
            [Display(Name = "Delete Quote")]
            public const string Delete = "Permissions.QuotesPermissions.Delete";
        }

        [DisplayName("Procedure")]
        [Description("Procedure Permissions")]
        public static class ProcedurePermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.ProcedurePermissions.All";
            [Display(Name = "Procedure View")]
            public const string View = "Permissions.ProcedurePermissions.View";
            [Display(Name = "Create Procedure")]
            public const string Create = "Permissions.ProcedurePermissions.Create";
            [Display(Name = "Edit Procedure")]
            public const string Edit = "Permissions.ProcedurePermissions.Edit";
            [Display(Name = "Delete Procedure")]
            public const string Delete = "Permissions.ProcedurePermissions.Delete";

        }

        [DisplayName("Support")]
        [Description("Support Permissions")]
        public static class SupportPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.SupportPermissions.All";
            [Display(Name = "Support View")]
            public const string View = "Permissions.SupportPermissions.View";
            [Display(Name = "Create Support")]
            public const string Create = "Permissions.SupportPermissions.Create";
            [Display(Name = "Edit Support")]
            public const string Edit = "Permissions.SupportPermissions.Edit";
            [Display(Name = "Delete Support")]
            public const string Delete = "Permissions.SupportPermissions.Delete";
        }

        [DisplayName("Support Response")]
        [Description("Support Response Permissions")]
        public static class SupportResponsePermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.SupportResponsePermissions.All";
            [Display(Name = "Support Response View")]
            public const string View = "Permissions.SupportResponsePermissions.View";
            [Display(Name = "Create Support Response")]
            public const string Create = "Permissions.SupportResponsePermissions.Create";
            [Display(Name = "Edit Support Response")]
            public const string Edit = "Permissions.SupportResponsePermissions.Edit";
            [Display(Name = "Delete Support Response")]
            public const string Delete = "Permissions.SupportResponsePermissions.Delete";
        }

        [DisplayName("Work Request")]
        [Description("Work Request Permissions")]
        public static class WorkRequestPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.WorkRequestPermissions.All";
            [Display(Name = "Work Request View")]
            public const string View = "Permissions.WorkRequestPermissions.View";
            [Display(Name = "Create Work Request")]
            public const string Create = "Permissions.WorkRequestPermissions.Create";
            [Display(Name = "Edit Work Request")]
            public const string Edit = "Permissions.WorkRequestPermissions.Edit";
            [Display(Name = "Delete Work Request")]
            public const string Delete = "Permissions.WorkRequestPermissions.Delete";
        }

        [DisplayName("System Preferences")]
        [Description("System Preferences Permissions")]
        public static class SystemPreferencesPermissions
        {
            [Display(Name = "All")]
            public const string All = "Permissions.SystemPreferencesPermissions.All";
        }

        

        /// <summary>
        /// Returns a list of Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRegisteredPermissions()
        {
            var permissions = new List<string>();
            foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    permissions.Add(propertyValue.ToString());
                }
            }
            return permissions;
        }

    }
}