﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FrontendWPF.ServiceStorageManger {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Response_Stock", Namespace="http://schemas.datacontract.org/2004/07/Base_service.JsonClasses")]
    [System.SerializableAttribute()]
    public partial class Response_Stock : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private FrontendWPF.ServiceStorageManger.Stock[] StocksField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public FrontendWPF.ServiceStorageManger.Stock[] Stocks {
            get {
                return this.StocksField;
            }
            set {
                if ((object.ReferenceEquals(this.StocksField, value) != true)) {
                    this.StocksField = value;
                    this.RaisePropertyChanged("Stocks");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Stock", Namespace="http://schemas.datacontract.org/2004/07/Base_service.JsonClasses")]
    [System.SerializableAttribute()]
    public partial class Stock : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<int> IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LocationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ProductField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<int> QuantityField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<int> Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Location {
            get {
                return this.LocationField;
            }
            set {
                if ((object.ReferenceEquals(this.LocationField, value) != true)) {
                    this.LocationField = value;
                    this.RaisePropertyChanged("Location");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Product {
            get {
                return this.ProductField;
            }
            set {
                if ((object.ReferenceEquals(this.ProductField, value) != true)) {
                    this.ProductField = value;
                    this.RaisePropertyChanged("Product");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<int> Quantity {
            get {
                return this.QuantityField;
            }
            set {
                if ((this.QuantityField.Equals(value) != true)) {
                    this.QuantityField = value;
                    this.RaisePropertyChanged("Quantity");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Response_Product", Namespace="http://schemas.datacontract.org/2004/07/Base_service.JsonClasses")]
    [System.SerializableAttribute()]
    public partial class Response_Product : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private FrontendWPF.ServiceStorageManger.Product[] ProductsField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public FrontendWPF.ServiceStorageManger.Product[] Products {
            get {
                return this.ProductsField;
            }
            set {
                if ((object.ReferenceEquals(this.ProductsField, value) != true)) {
                    this.ProductsField = value;
                    this.RaisePropertyChanged("Products");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Product", Namespace="http://schemas.datacontract.org/2004/07/Base_service.JsonClasses")]
    [System.SerializableAttribute()]
    public partial class Product : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<int> IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<int> UnitPriceField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<int> Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<int> UnitPrice {
            get {
                return this.UnitPriceField;
            }
            set {
                if ((this.UnitPriceField.Equals(value) != true)) {
                    this.UnitPriceField = value;
                    this.RaisePropertyChanged("UnitPrice");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Response_SalePurchase", Namespace="http://schemas.datacontract.org/2004/07/Base_service.JsonClasses")]
    [System.SerializableAttribute()]
    public partial class Response_SalePurchase : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private FrontendWPF.ServiceStorageManger.SalePurchase[] SalesPurchasesField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public FrontendWPF.ServiceStorageManger.SalePurchase[] SalesPurchases {
            get {
                return this.SalesPurchasesField;
            }
            set {
                if ((object.ReferenceEquals(this.SalesPurchasesField, value) != true)) {
                    this.SalesPurchasesField = value;
                    this.RaisePropertyChanged("SalesPurchases");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="SalePurchase", Namespace="http://schemas.datacontract.org/2004/07/Base_service.JsonClasses")]
    [System.SerializableAttribute()]
    public partial class SalePurchase : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<System.DateTime> DateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<int> IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LocationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ProductField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<int> QuantityField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UsernameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> Date {
            get {
                return this.DateField;
            }
            set {
                if ((this.DateField.Equals(value) != true)) {
                    this.DateField = value;
                    this.RaisePropertyChanged("Date");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<int> Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Location {
            get {
                return this.LocationField;
            }
            set {
                if ((object.ReferenceEquals(this.LocationField, value) != true)) {
                    this.LocationField = value;
                    this.RaisePropertyChanged("Location");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Product {
            get {
                return this.ProductField;
            }
            set {
                if ((object.ReferenceEquals(this.ProductField, value) != true)) {
                    this.ProductField = value;
                    this.RaisePropertyChanged("Product");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<int> Quantity {
            get {
                return this.QuantityField;
            }
            set {
                if ((this.QuantityField.Equals(value) != true)) {
                    this.QuantityField = value;
                    this.RaisePropertyChanged("Quantity");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Username {
            get {
                return this.UsernameField;
            }
            set {
                if ((object.ReferenceEquals(this.UsernameField, value) != true)) {
                    this.UsernameField = value;
                    this.RaisePropertyChanged("Username");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceStorageManger.IStockService")]
    public interface IStockService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/ListStock", ReplyAction="http://tempuri.org/IStockService/ListStockResponse")]
        FrontendWPF.ServiceStorageManger.Response_Stock ListStock(string uid, string id, string product, string location, string qOver, string qUnder, string limit);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/ListStock", ReplyAction="http://tempuri.org/IStockService/ListStockResponse")]
        System.Threading.Tasks.Task<FrontendWPF.ServiceStorageManger.Response_Stock> ListStockAsync(string uid, string id, string product, string location, string qOver, string qUnder, string limit);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/AddStock", ReplyAction="http://tempuri.org/IStockService/AddStockResponse")]
        string AddStock(string uid, string product, string location);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/AddStock", ReplyAction="http://tempuri.org/IStockService/AddStockResponse")]
        System.Threading.Tasks.Task<string> AddStockAsync(string uid, string product, string location);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/UpdateStock", ReplyAction="http://tempuri.org/IStockService/UpdateStockResponse")]
        string UpdateStock(string uid, string id, string product, string location, string quantity);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/UpdateStock", ReplyAction="http://tempuri.org/IStockService/UpdateStockResponse")]
        System.Threading.Tasks.Task<string> UpdateStockAsync(string uid, string id, string product, string location, string quantity);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/RemoveStock", ReplyAction="http://tempuri.org/IStockService/RemoveStockResponse")]
        string RemoveStock(string uid, string id, string location);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/RemoveStock", ReplyAction="http://tempuri.org/IStockService/RemoveStockResponse")]
        System.Threading.Tasks.Task<string> RemoveStockAsync(string uid, string id, string location);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/ListProduct", ReplyAction="http://tempuri.org/IStockService/ListProductResponse")]
        FrontendWPF.ServiceStorageManger.Response_Product ListProduct(string uid, string id, string name, string qOver, string qUnder, string limit);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/ListProduct", ReplyAction="http://tempuri.org/IStockService/ListProductResponse")]
        System.Threading.Tasks.Task<FrontendWPF.ServiceStorageManger.Response_Product> ListProductAsync(string uid, string id, string name, string qOver, string qUnder, string limit);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/AddProduct", ReplyAction="http://tempuri.org/IStockService/AddProductResponse")]
        string AddProduct(string uid, string name, string unitPrice);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/AddProduct", ReplyAction="http://tempuri.org/IStockService/AddProductResponse")]
        System.Threading.Tasks.Task<string> AddProductAsync(string uid, string name, string unitPrice);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/UpdateProduct", ReplyAction="http://tempuri.org/IStockService/UpdateProductResponse")]
        string UpdateProduct(string uid, string id, string name, string unitPrice);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/UpdateProduct", ReplyAction="http://tempuri.org/IStockService/UpdateProductResponse")]
        System.Threading.Tasks.Task<string> UpdateProductAsync(string uid, string id, string name, string unitPrice);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/RemoveProduct", ReplyAction="http://tempuri.org/IStockService/RemoveProductResponse")]
        string RemoveProduct(string uid, string id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/RemoveProduct", ReplyAction="http://tempuri.org/IStockService/RemoveProductResponse")]
        System.Threading.Tasks.Task<string> RemoveProductAsync(string uid, string id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/ListSalePurchase", ReplyAction="http://tempuri.org/IStockService/ListSalePurchaseResponse")]
        FrontendWPF.ServiceStorageManger.Response_SalePurchase ListSalePurchase(string uid, string type, string id, string product, string qOver, string qUnder, string before, string after, string location, string username, string limit);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/ListSalePurchase", ReplyAction="http://tempuri.org/IStockService/ListSalePurchaseResponse")]
        System.Threading.Tasks.Task<FrontendWPF.ServiceStorageManger.Response_SalePurchase> ListSalePurchaseAsync(string uid, string type, string id, string product, string qOver, string qUnder, string before, string after, string location, string username, string limit);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/AddSalePurchase", ReplyAction="http://tempuri.org/IStockService/AddSalePurchaseResponse")]
        string AddSalePurchase(string uid, string type, string product, string quantity, string location, string date);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/AddSalePurchase", ReplyAction="http://tempuri.org/IStockService/AddSalePurchaseResponse")]
        System.Threading.Tasks.Task<string> AddSalePurchaseAsync(string uid, string type, string product, string quantity, string location, string date);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/UpdateSalePurchase", ReplyAction="http://tempuri.org/IStockService/UpdateSalePurchaseResponse")]
        string UpdateSalePurchase(string uid, string id, string type, string product, string quantity, string date, string location, string username);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/UpdateSalePurchase", ReplyAction="http://tempuri.org/IStockService/UpdateSalePurchaseResponse")]
        System.Threading.Tasks.Task<string> UpdateSalePurchaseAsync(string uid, string id, string type, string product, string quantity, string date, string location, string username);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/RemoveSalePurchase", ReplyAction="http://tempuri.org/IStockService/RemoveSalePurchaseResponse")]
        string RemoveSalePurchase(string uid, string type, string id, string location);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IStockService/RemoveSalePurchase", ReplyAction="http://tempuri.org/IStockService/RemoveSalePurchaseResponse")]
        System.Threading.Tasks.Task<string> RemoveSalePurchaseAsync(string uid, string type, string id, string location);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IStockServiceChannel : FrontendWPF.ServiceStorageManger.IStockService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class StockServiceClient : System.ServiceModel.ClientBase<FrontendWPF.ServiceStorageManger.IStockService>, FrontendWPF.ServiceStorageManger.IStockService {
        
        public StockServiceClient() {
        }
        
        public StockServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public StockServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public StockServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public StockServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public FrontendWPF.ServiceStorageManger.Response_Stock ListStock(string uid, string id, string product, string location, string qOver, string qUnder, string limit) {
            return base.Channel.ListStock(uid, id, product, location, qOver, qUnder, limit);
        }
        
        public System.Threading.Tasks.Task<FrontendWPF.ServiceStorageManger.Response_Stock> ListStockAsync(string uid, string id, string product, string location, string qOver, string qUnder, string limit) {
            return base.Channel.ListStockAsync(uid, id, product, location, qOver, qUnder, limit);
        }
        
        public string AddStock(string uid, string product, string location) {
            return base.Channel.AddStock(uid, product, location);
        }
        
        public System.Threading.Tasks.Task<string> AddStockAsync(string uid, string product, string location) {
            return base.Channel.AddStockAsync(uid, product, location);
        }
        
        public string UpdateStock(string uid, string id, string product, string location, string quantity) {
            return base.Channel.UpdateStock(uid, id, product, location, quantity);
        }
        
        public System.Threading.Tasks.Task<string> UpdateStockAsync(string uid, string id, string product, string location, string quantity) {
            return base.Channel.UpdateStockAsync(uid, id, product, location, quantity);
        }
        
        public string RemoveStock(string uid, string id, string location) {
            return base.Channel.RemoveStock(uid, id, location);
        }
        
        public System.Threading.Tasks.Task<string> RemoveStockAsync(string uid, string id, string location) {
            return base.Channel.RemoveStockAsync(uid, id, location);
        }
        
        public FrontendWPF.ServiceStorageManger.Response_Product ListProduct(string uid, string id, string name, string qOver, string qUnder, string limit) {
            return base.Channel.ListProduct(uid, id, name, qOver, qUnder, limit);
        }
        
        public System.Threading.Tasks.Task<FrontendWPF.ServiceStorageManger.Response_Product> ListProductAsync(string uid, string id, string name, string qOver, string qUnder, string limit) {
            return base.Channel.ListProductAsync(uid, id, name, qOver, qUnder, limit);
        }
        
        public string AddProduct(string uid, string name, string unitPrice) {
            return base.Channel.AddProduct(uid, name, unitPrice);
        }
        
        public System.Threading.Tasks.Task<string> AddProductAsync(string uid, string name, string unitPrice) {
            return base.Channel.AddProductAsync(uid, name, unitPrice);
        }
        
        public string UpdateProduct(string uid, string id, string name, string unitPrice) {
            return base.Channel.UpdateProduct(uid, id, name, unitPrice);
        }
        
        public System.Threading.Tasks.Task<string> UpdateProductAsync(string uid, string id, string name, string unitPrice) {
            return base.Channel.UpdateProductAsync(uid, id, name, unitPrice);
        }
        
        public string RemoveProduct(string uid, string id) {
            return base.Channel.RemoveProduct(uid, id);
        }
        
        public System.Threading.Tasks.Task<string> RemoveProductAsync(string uid, string id) {
            return base.Channel.RemoveProductAsync(uid, id);
        }
        
        public FrontendWPF.ServiceStorageManger.Response_SalePurchase ListSalePurchase(string uid, string type, string id, string product, string qOver, string qUnder, string before, string after, string location, string username, string limit) {
            return base.Channel.ListSalePurchase(uid, type, id, product, qOver, qUnder, before, after, location, username, limit);
        }
        
        public System.Threading.Tasks.Task<FrontendWPF.ServiceStorageManger.Response_SalePurchase> ListSalePurchaseAsync(string uid, string type, string id, string product, string qOver, string qUnder, string before, string after, string location, string username, string limit) {
            return base.Channel.ListSalePurchaseAsync(uid, type, id, product, qOver, qUnder, before, after, location, username, limit);
        }
        
        public string AddSalePurchase(string uid, string type, string product, string quantity, string location, string date) {
            return base.Channel.AddSalePurchase(uid, type, product, quantity, location, date);
        }
        
        public System.Threading.Tasks.Task<string> AddSalePurchaseAsync(string uid, string type, string product, string quantity, string location, string date) {
            return base.Channel.AddSalePurchaseAsync(uid, type, product, quantity, location, date);
        }
        
        public string UpdateSalePurchase(string uid, string id, string type, string product, string quantity, string date, string location, string username) {
            return base.Channel.UpdateSalePurchase(uid, id, type, product, quantity, date, location, username);
        }
        
        public System.Threading.Tasks.Task<string> UpdateSalePurchaseAsync(string uid, string id, string type, string product, string quantity, string date, string location, string username) {
            return base.Channel.UpdateSalePurchaseAsync(uid, id, type, product, quantity, date, location, username);
        }
        
        public string RemoveSalePurchase(string uid, string type, string id, string location) {
            return base.Channel.RemoveSalePurchase(uid, type, id, location);
        }
        
        public System.Threading.Tasks.Task<string> RemoveSalePurchaseAsync(string uid, string type, string id, string location) {
            return base.Channel.RemoveSalePurchaseAsync(uid, type, id, location);
        }
    }
}
