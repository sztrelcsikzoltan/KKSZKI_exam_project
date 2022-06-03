import React from 'react';
import {Base_storage, User} from '../Variables';
import { BaseModal } from './BaseModal';

export function CreateWindow({onClose}) {

    function Create(e)
    {
        e.preventDefault();
        fetch(Base_storage + "AddProduct",
        { 
            method: "POST",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                name: e.target.elements.name.value,
                buyUnitPrice: e.target.elements.purchasePrice.value,
                sellUnitPrice: e.target.elements.salesPrice.value
            })
        })
        .then(res => res.json())
        .then((response) =>{
            alert(response);
            if(response === "X"){
                alert("This is a specifice alert!");
                return;
            }
            else{
                window.location.reload();
            }
        })
        .catch((response) => {
            console.log(response);
            return;
        });
    }

    return (
		<BaseModal Submitted={Create} onClosed={onClose}  title="Create Product">
			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Name:</label>
                <div className="col-sm-9">
                    <input type="text" name="name" className="form-control" />
                </div>
            </div>
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Purchase price:</label>
                <div className="col-sm-9">
                    <input type="number" name="purchasePrice" className="form-control" min="0" max="10000000" />
                </div>
            </div>	
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Sales price:</label>
                <div className="col-sm-9">
                    <input type="number" name="salesPrice" className="form-control" min="0" max="10000000" />
                </div>
            </div>
		add product?
        </BaseModal>
    )
}

export function UpdateWindow({onClose, product}){

    function Update(e){
        e.preventDefault();
		fetch(Base_storage + "UpdateProduct",
		{ 
            method: "PUT",
			headers:{'Content-Type': 'application/json'},
			body: JSON.stringify({
				uid: User.Uid,
				id: product.Id,
				name: e.target.elements.name.value,
				buyUnitPrice: e.target.elements.purchasePrice.value,
				sellUnitPrice: e.target.elements.salesPrice.value
		    })
		})
		.then(res => res.json())
		.then((response) =>{
			alert(response);
			if(response === "X"){
				alert("This is a specifice alert!");
				return;
			}
            else{
                window.location.reload();
            }
		})
		.catch((response) => {
			console.log(response);
			return;
		});
    }

    return(
		<BaseModal Submitted={Update} onClosed={onClose} title="Update Product">
			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Name:</label>
                <div className="col-sm-9">
                    <input type="text" name="name" className="form-control" defaultValue={product == null ? "" : product.Name} />
                </div>
            </div>
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Purchase price:</label>
                <div className="col-sm-9">
                    <input type="number" name="purchasePrice" className="form-control" defaultValue={product == null ? "" : product.BuyUnitPrice} />
                </div>
            </div>	
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Sales price:</label>
                <div className="col-sm-9">
                    <input type="number" name="salesPrice" className="form-control" defaultValue={product == null ? "" : product.SellUnitPrice}/>
                </div>
            </div>
		update product?
		</BaseModal>
    )
}

export function DeleteWindow({onClose, product}) {

    function Delete(e){
        e.preventDefault();
        fetch(Base_storage + "DeleteProduct",
        { 
            method: "DELETE",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                id: product.Id
            })
        })
        .then(res => res.json())
        .then((response) =>{
            alert(response);
            if(response === "X"){
                alert("This is a specifice alert!");
                return;
            }
            else{
                window.location.reload();
            }
        })
        .catch((response) => {
            console.log(response);
            return;
        });
    }

    return(
        <BaseModal Submitted={Delete} onClosed={onClose}  title="Delete Product">
            Are you sure to delete the product <span><b>{product == null ? "" : product.Name}</b></span> ?
        </BaseModal>
    )
}