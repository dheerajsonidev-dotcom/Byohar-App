namespace Byohar.Shared.Constants.Api
{
    public static class ApiConstants
    {
        public const string GetAll = "get-all";
        public const string GetSingle = "get-data";
        public const string GetAllByBuildingId = "get-all-by-buildingId";
        public const string Create = "create";
        public const string Update = "update";
        public const string DeleteMany = "delete-many";
        public const string DeleteAll = "delete-all";
        public const string ExportExcel = "export-to-excel";
        public const string ExportPdf = "export-to-pdf";
        public const string ExportCsv = "export-to-csv";  
        public const string BulkUpload = "bulk-upload";

        public static class ViewState
        {
            public const string GetByEntityType = "by-entity-type";
        }

        public static class RequiredFields
        {
            public const string GetFields = "get-fields";
        }

        public static class Supplier
        {
            public const string UpdateStatus = "update-status";
        }

        public static class Asset
        {
            public const string UpdateHealthDetails = "update-health-details";
            public const string UpdateHierarchy= "update-hierarchy";
            public const string UpdateManyAsset = "update-many-asset";
            public const string GetAllByTasks = "get-all-by-tasks";
            public const string GetLifeSpan = "life-span-count";
            public const string GetLifeSpanByBuilding = "life-span-by-building-count";
            public const string GetReplacementForecast = "replacement";
            public const string GetReplacementForecastByYear = "replacement-by-year";
            public const string GetOverallCompliance = "overall-compliance";
            public const string ExportReplacementForecastPdf = "replacement-forecast-export-to-pdf";
        }

        public static class AssetCondition
        {
            public const string AssetByCondition = "asset-by-condition";
        }

        public static class BuildingGeneralDetails
        {
            public const string Create = "add-general-details";
            public const string Update = "update-general-details";
        }

        public static class Building
        {
            public const string ByWorkOrders = "by-workorders";
        }

        public static class Documents
        {
            public const string DeleteFile = "file";
        }

        public static class Skills
        {
            public const string DeleteAll = "delete-all-skills";
            public const string DeleteMany = "delete-many-skills";
        }

        public static class Qualifications
        {
            public const string DeleteAll = "delete-all-qualifications";
            public const string DeleteMany = "delete-many-qualifications";
        }

        public static class Technicians
        {
            public const string UpdateStatus = "update-status";
            public static class Buildings
            {
                public const string DeleteAll = "delete-all-buildings";
                public const string DeleteMany = "delete-many-buildings";
                public const string Create = "create-technician-building";
            }

            public static class Qualifications
            {
                public const string GetAll = "get-all-qualifications";
                public const string Update = "update-qualification";
            }

            public static class Skills
            {
                public const string GetAll = "get-all-skills";
                public const string Update = "update-skills";
            }
        }

        public static class WorkOrder
        {
            public const string CreateWOProcedure = "create-work-order-procedure";
            public const string SendWorkOrderEmail = "send-work-order-email";
            public const string CustomerSatisfactionForm = "customer-satisfaction-form";
            public const string CustomerSatisfaction= "customer-satisfaction";
            public const string Status = "by-status";
            public const string Count = "count";
            public const string GetCombinedPPMAndWorkOrder = "combined-ppm-work-order";
            public const string ExportServiceHistoryPdf = "service-history-to-pdf";
            public const string CompletedByMonth = "ppm-work-order-by-month";
        }

        public static class PurchaseOrder
        {
            public const string SendPurchaseOrderEmail = "send-purchase-order-email";
        }

        public static class DirectMessage
        {
            public const string ByUserId = "get-by-userId";
        }

        public static class Notification
        {
            public const string MarkAsRead = "mark-as-read";
        }

        public static class Group
        {
            public const string LeaveGroup = "leave";
        }

        public static class Procedure
        {
            public const string GetById = "get-by-id";
            public const string UpdateQuestions = "update-questions";
            public const string GetAllResponses = "get-all-responses";
        }

        public static class User
        {
            public const string DeleteAllRoles = "user-role/delete-all/{userId}";
            public const string DeleteRoles = "user-role/delete-many";
        }

        public static class Expense
        {
            public const string ByCostCode = "cost-code";
            public const string ByCostCentre = "cost-centre";
            public const string ByBuilding = "building";
            public const string ByMaintenance = "maintenance";
        }

        public static class PPM
        {
            public const string GetCompletedAndDuePPM = "completed-due";
            public const string GetPastPPM = "get-past-ppm";
            public const string DeleteManyPPMStatusTracker = "delete-many-ppm-status";
            public const string DeleteAllPPMStatusTracker = "delete-all-ppm-status";
            public const string SendPPMEmail = "send-ppm-email";
            public const string CompletedByMonth = "completed-by-month";
        }

        public static class WorkRequest
        {
            public const string Count = "count";
            public const string CountByMonth = "count-by-month";
        }

        public static class Invoice
        {
            public const string CreateXeroInvoice = "create-xero-invoice";
        }

        public static class Priority
        {
            public const string CheckPriorityRank = "check-priority-rank";
        }
    }
}