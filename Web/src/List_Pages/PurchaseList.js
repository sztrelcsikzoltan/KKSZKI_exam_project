import { useState, useEffect, Fragment } from 'react';
import { useNavigate } from "react-router-dom";
import { Base_storage, User } from '../Variables';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus, faPencilAlt, faTrash } from "@fortawesome/free-solid-svg-icons";
import { CreateWindow, UpdateWindow, DeleteWindow } from "../Windows/PurchaseWindows"

function Purchases({currentpage}) {
    const [purchases, setPurchases] = useState([]);
    const navigate = useNavigate();

    useEffect(() =>{
		//Check if session storage has user data and load it, if not, return user to login screen
        if(User.Uid === null || User.Uid === "")
        { 
            User.Uid = sessionStorage.getItem("Uid");
            User.Details = JSON.parse(sessionStorage.getItem("Details"));
            if(User.Uid === null || User.Uid === ""){
                console.log("User data could not be loaded from cache, redirecting!"); 
                navigate("/login");
            }
            else {console.log("User data loaded from cache!");}
        }
		//Set the page we are on, used for the selection on the navbar
		currentpage("purchase");

		//Basic fetch of everything in table
        fetch(Base_storage + "listsalepurchase?uid=" + User.Uid + "&type=purchase", {
            method:"GET"
        })
        .then(res => res.json())
        .then((response) =>{
            console.log(response.Message);

			//Set order of items to descending based on Id
            setPurchases(response.SalesPurchases.sort(function(a, b){
                return b.Id - a.Id;
            }).slice(0, 50));
        })
        .catch((response) => {
            console.log(response.Message);
        });
    }, [navigate, currentpage]);

	//A secondary, more complicated fetch using the form inputs above the list of items
    function SearchSubmit(e){
        e.preventDefault();
        e = e.target.elements;
        fetch(Base_storage + "listsalepurchase?uid=" + User.Uid + "&type=purchase&product=" + e.product.value + "&quantityover=" + e.qover.value + 
            "&quantityunder=" + e.qunder.value + "&priceover=" + e.pover.value + "&priceunder=" + e.punder.value + "&before=" + e.before.value + "&after=" + e.after.value +
            "&location=" + e.location.value + "&username=" + e.username.value + "&limit=" + e.limit.value
        ,{
            method:"GET"
        })
        .then(res => res.json())
        .then((response) =>{
            console.log(response.Message);
            
			//Set order of items to descending based on Id
            setPurchases(response.SalesPurchases.sort(function(a, b){
                return b.Id - a.Id;
            }));
        })
        .catch((response) => {
            console.log(response.Message);
        });
    }


	//Constants used for the update/add/remove pop up windows
    const [deletedisplay, setDeleteDisplay] = useState(false);
    const [createdisplay, setCreateDisplay] = useState(false);
    const [updatedisplay, setUpdateDisplay] = useState(false);
    const [purchase, setPurchase] = useState();

	//Start process of updating the item where we clicked on the button
    function Update(e){
        var element;
		//check what part of the button we pressed
		//as depending on the part, we have different amounts of parent nodes to get through
        if(e.target.tagName === "svg"){
            element = e.target.parentNode;
        }
        else if(e.target.tagName === "path"){
            element = e.target.parentNode.parentNode;
        }
        else element = e.target;

		//get to the div that is the parent of an entire list item, and get it's first child, which contains the id of the item
		//Search for it in the array and set the constants accordingly
        var currid = element.parentNode.parentNode.firstChild.innerText;
        var purchase = purchases.find((pur) => pur.Id === parseInt(currid));
        if(purchase != null){
            setPurchase(purchase);
            setUpdateDisplay(true);
        }
    }

	//Start process of deleting the item where we clicked on the button
    function Delete(e){
        var element;
		//check what part of the button we pressed
		//as depending on the part, we have different amounts of parent nodes to get through
        if(e.target.tagName === "svg"){
            element = e.target.parentNode;
        }
        else if(e.target.tagName === "path"){
            element = e.target.parentNode.parentNode;
        }
        else element = e.target;

		//get to the div that is the parent of an entire list item, and get it's first child, which contains the id of the item
		//Search for it in the array and set the constants accordingly
        var currid = element.parentNode.parentNode.firstChild.innerText;
        var purchase = purchases.find((pur) => pur.Id === parseInt(currid));
        if(purchase != null){
            setPurchase(purchase);
            setDeleteDisplay(true);
        }
    }

    return (
        <Fragment>
            <div className='res-primary res-background'>
                <div className="row border-bottom py-2 w-100">

                    <h5 className="col-xs-12 col-sm-1">Id</h5>
                    <h5 className="col-xs-12 col-sm-1">Product</h5>
                    <h5 className="col-xs-12 col-sm-2">Quantity</h5>
                    <h5 className="col-xs-12 col-sm-2">Total Price</h5>
                    <h5 className="col-xs-12 col-sm-2">Date</h5>
                    <h5 className="col-xs-12 col-sm-1">Location</h5>
                    <h5 className="col-xs-12 col-sm-2">User</h5>
                    <div className="col-xs-10 col-sm-1 ">
					  	<button className={"btn btn-sm btn-outline-warning mr-2 " + (User.Details.Permission > 2 ? "" : "invisible")} 
					     onClick={() => {setCreateDisplay(true)}}>
							{<FontAwesomeIcon icon={faPlus} />}
						</button>  
				  	</div>

                </div>


                <form onSubmit={SearchSubmit} id="filterForm">
                    <div className="row border-bottom py-2 w-100">

                        <div className='col-xs-12 col-sm-1'>
                            <button type="submit" className='btn btn-dark'>Filter</button>
                            <button type="Reset" className='btn btn-dark'>Reset</button>
                        </div>

                        <div className="col-xs-12 col-sm-1">
                            <label className='form-label mx-2' htmlFor="product">Name:</label>
                            <input type="text" name="product" className='w-50'/>
                        </div>

                        <div className="col-xs-12 col-sm-2">
                            <div className=' float-start'>
                                <label className='form-label me-1' htmlFor="qover">Over:</label>
                                <input type="number" name="qover" className='form-text w-25'/>
                                <label className='form-label ms-2 me-1' htmlFor="qunder">Under:</label>
                                <input type="number" name="qunder" className='form-text w-25'/>
                            </div>
                        </div>

                        <div className="col-xs-12 col-sm-2">
                            <div className=' float-start'>
                                <label className='form-label me-1' htmlFor="pover">Over:</label>
                                <input type="number" name="pover" className='form-text w-25'/>
                                <label className='form-label ms-2 me-1' htmlFor="punder">Under:</label>
                                <input type="number" name="punder" className='form-text w-25'/>
                            </div>
                        </div>

                        <div className="col-xs-12 col-sm-2">
                            <div className=' float-start'>
                                <label className='form-label me-1' htmlFor="before">Before:</label>
                                <input type="date" name="before" className='form-text'/><br/>
                                <label className='form-label ms-2 me-1' htmlFor="after">After:</label>
                                <input type="date" name="after" className='form-text'/>
                            </div>
                        </div>

                        <div className="col-xs-12 col-sm-1">
                            <label className='form-label mx-2' htmlFor="location">Name:</label>
                            <input type="text" name="location" className='w-50'/>
                        </div>

                        <div className="col-xs-12 col-sm-2">
                            <label className='form-label mx-2' htmlFor="username">Userame:</label>
                            <input type="text" name="username" className='w-50'/>
                        </div> 

                        <div className='col-xs-12 col-sm-1'>
                            <label className='form-label mx-2' htmlFor="limit">Limit:</label>
                            <input type="number" name="limit" className='form-text w-50'></input>
                        </div>

                    </div>
                </form>


                <div className="list res-background">

                    {purchases.map((purchase) => (
                        <div key={purchase.Id} className="row border-bottom  py-2 w-100">
                            <div className="col-xs-12 col-sm-1">{purchase.Id}</div>
                            <div className="col-xs-4 col-sm-1">{purchase.Product}</div>
                            <div className="col-xs-8 col-sm-2">{purchase.Quantity}</div>
                            <div className="col-xs-10 col-sm-2">{purchase.TotalPrice}</div>
                            <div className="col-xs-10 col-sm-2">{new Date(parseInt(purchase.Date.substring(6,19))).toDateString()}</div>
                            <div className="col-xs-10 col-sm-1">{purchase.Location}</div>
                            <div className="col-xs-10 col-sm-2">{purchase.Username}</div>
                            <div className="col-xs-10 col-sm-1">
							    <button className={"btn btn-sm btn-outline-warning mr-2 " + (User.Details.Permission > 2 ? "" : "invisible")} onClick={Update}>	
						  		    {<FontAwesomeIcon icon={faPencilAlt} />}
					  	  	    </button>  	
						  	    <button className={"btn btn-sm btn-outline-warning mr-2 " + (User.Details.Permission > 5 ? "" : "invisible")} onClick={Delete} >
								    {<FontAwesomeIcon icon={faTrash} />}
							    </button>  
						    </div>
                        </div>
                    ))}

                </div>
            </div>
        
		    <div className={createdisplay ? "" : "invisible"}><CreateWindow onClose={() => {setCreateDisplay(false)}}/></div>
		    <div className={updatedisplay ? "" : "invisible"}><UpdateWindow purchase={purchase} onClose={() => {setUpdateDisplay(false)}}/></div>
		    <div className={deletedisplay ? "" : "invisible"}><DeleteWindow purchase={purchase} onClose={() => {setDeleteDisplay(false)}}/></div>
        </Fragment>
    )
}

export default Purchases