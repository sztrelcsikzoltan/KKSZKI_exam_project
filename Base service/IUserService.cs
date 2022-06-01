﻿using System.Collections.Generic;
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
            UriTemplate = "/user/list?id={id}&username={username}&location={location}&region={region}&limit={limit}"
            )]
        Response ListUser(string id, string username, string location, string region, string limit);

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/user/login?username={username}&password={password}"
            )]
        Response LoginUser(string username, string password);

        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/user/register?username={username}&password={password}&locationid={locationId}&permission={permission}"
            )]
        //id and active status is given by default, so those do not need to be given
        string RegisterUser(string username, string password, string locationId, string permission);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/user/update?id={id}&username={username}&password={password}&locationid={locationId}&permission={permission}&active={active}"
            )]
        //You can't update the id, it is used to search for the user to be updated
        string UpdateUser(string id, string username, string password, string locationId, string permission, string active);

        [OperationContract]
        [WebInvoke(Method = "DELETE",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "/user/delete?id={id}"
            )]
        string DeleteUser(string id);
    }

    [DataContract]
    public class Response
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

        public Response() { }

        public Response(string message, List<User> users)
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