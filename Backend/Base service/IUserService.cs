﻿using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Base_service
{
    [ServiceContract]
    public interface IUserManagement
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/list?id={id}&username={username}&location={location}&region={region}&limit={limit}"
            )]
        Response_User ListUser([Optional] string id, [Optional] string username, [Optional] string location, [Optional] string region, [Optional] string limit);

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/login?username={username}&password={password}"
            )]
        Response_Login LoginUser(string username, string password);

        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/register?username={username}&password={password}&locationid={locationId}&permission={permission}"
            )]
        //id and active status is given by default, so those do not need to be given
        string RegisterUser(string username, string password, string locationId, string permission);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/update?id={id}&username={username}&password={password}&locationid={locationId}&permission={permission}&active={active}"
            )]
        //You can't update the id, it is used to search for the user to be updated
        string UpdateUser(string id, [Optional] string username, [Optional] string password, [Optional] string locationId, [Optional] string permission, [Optional] string active);

        [OperationContract]
        [WebInvoke(Method = "DELETE",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/delete?id={id}"
            )]
        string DeleteUser(string id);
    }

    [DataContract]
    public class Response_User
    {
        private string message = null;
        private List<User> users= new List<User>();

        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        [DataMember]
        public List<User> Users
        {
            get { return users; }
            set { users = value; }
        }

        public Response_User() { }

        public Response_User(string message, List<User> users)
        {
            Message = message;
            Users = users;
        }
    }

    [DataContract]
    public class Response_Login
    {
        private string message = null, uid = null;
        private List<User> users = new List<User>();

        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        [DataMember]
        public string Uid
        {
            get { return uid; }
            set { uid = value; }
        }

        [DataMember]
        public List<User> Users
        {
            get { return users; }
            set { users = value; }
        }

        public Response_Login() { }

        public Response_Login(string message, List<User> users)
        {
            Message = message;
            Users = users;
        }
    }

    [DataContract]
    public class User
    {
        private string username, password, location = null;
        private int? id, permission, active = null;

        [DataMember]
        public int? Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        [DataMember]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [DataMember]
        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        [DataMember]
        public int? Permission
        {
            get { return permission; }
            set { permission = value; }
        }

        [DataMember]
        public int? Active
        {
            get { return active; }
            set { active = value; }
        }

        public User() { }

        public User(int? id, string username, string password, string location, int? permission, int? active)
        {
            Id = id;
            Username = username;
            Password = password;
            Location = location;
            Permission = permission;
            Active = active;
        }
    }
}
