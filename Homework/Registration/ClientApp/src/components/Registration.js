import React, { Component } from 'react';
import Logo20 from './../imgs/logo20.png';
import RegBtn from './../imgs/RegisterButton.png';
import { MicrosoftLoginButton } from "react-social-login-buttons";
import { GoogleLoginButton } from "react-social-login-buttons";
import PhoneInput from 'react-phone-input-2';

// Add this in your component file
//require('react-dom');
//window.React2 = require('react');
//console.log(window.React1 === window.React2);
const re =
    /^(([^<>()[\]\.,;:\s@\"]+(\.[^<>()[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;

const numre = /^\d{10}$/;

const validateForm = (errs) => {
    let valid = true;
    Object.values(errs).forEach(val => val.length > 0 && (valid = false));
    return valid;
};

export class Registration extends Component {
    static displayName = Registration.name;

    constructor(props) {
        super(props);
        this.state = {
            fullname: null,
            email: null,
            password: null,
            npi: null,
            busaddress: null,
            phone: null,
            errors: {
                fullname: '',
                email: '',
                password: '',
                npi: '',
                busaddress: '',
                phone: ''
            }
        };
    }

    setValue = async (e) => {
        e.preventDefault();
        const { name, value } = e.target;
        let errors = this.state.errors;

        switch (name) {
            case 'fullname':
                errors.fullname =
                    value.length < 5
                        ? 'Full Name must be at least 5 characters long!'
                        : '';
                break;
            case 'email':
                errors.email =
                    re.test(value)
                        ? ''
                        : 'Email is not valid!';
                break;
            case 'password':
                errors.password =
                    value.length < 8
                        ? 'Password must be at least 8 characters long!'
                        : '';
                break;
            case 'npi':
                console.log('In the NPI section  ' + name);
                errors.npi =
                    ((/^\d+$/.test(value)) && value.length < 5) ? "NPI must be numeric and upto 10 characters long!" : "";
                e.target.value = (/^\d+$/.test(value)) ? value : "";
                break;
            case 'busaddress':
                errors.busaddress =
                    value.length < 8
                        ? 'Address must not be empty please provide an address 8 or more characters long!'
                        : '';
                break;
            case 'phone':
                errors.phone =
                    ((numre.match(value)) && value.length) < 10
                        ? 'Phone number must be present at least 10 or more characters long!'
                        : '';
                e.target.value = (numre.match(value)) ? value : "";
                break;
            default:
                break;
        }
        this.setState({ errors, [name]: value });
    }

    sendData = async () => {
        const response = await fetch('/api/registration', {
            method: 'PUT',
            body: JSON.stringify(this.state)
        });
        if (response.status !== 200) {
            throw new Error(`Request failed: ${response.status}`);
        }
    }

    initSubmit = async (e) => {
        e.preventDefault();
        console.log(this.state);

        try {
            if (validateForm(this.state.errors)) {
                await this.sendData;

                alert('Your registration was successfully submitted!');
                
                console.info('Valid Form');

                this.state.email = '';
                this.state.phone = '';
                this.state.fullname = '';
                this.state.password = '';
                this.state.busaddress = '';
                this.state.npi = '';
                document.getElementById("resgistrationForm").reset();
            }
            else {
                console.error('Invalid Form');
            }
        } catch (e) {
            alert(`Registration failed! ${e.message}`);
        }
    }

    render() {
        //const REACT_VERSION = React.version;
        //console.log(REACT_VERSION);
        return (
            <form id="resgistrationForm" onSubmit={(e) => this.setValue(e)}>
                <React.StrictMode>
                <div className="regControlBase">
                    <div className="companyInfo">
                        <h1><img src={Logo20} alt="Availity" /></h1>
                        <h3>If you are a medical billing services company, select this option.</h3>
                        <p>If your business submits claims or other transactions on behalf of one or more providers (provider groups) – click below to register. Questions about registering? Join us for a live webinar or explore other registration resources on our training microsite.</p>
                        <h3>Select this option if you are a healthcare provider.</h3>
                        <p>If you are a healthcare provider – i.e., physician practice, mental health provider, specialist, medical transportation service, or non-physician provider – click below to register. Questions about registering? Join us for a live webinar or explore other registration resources on our training microsite.</p>
                    </div>
                    <div className="regFormBody">
                        <h4>Registration</h4>
                        <p>Don't have an account create your account, it takes less than a minute.</p>
                        <ul>
                            <li>
                                <input type="text" name="fullname" className="inputRegister" placeholder="First and Last Name" onChange={(e) => this.setValue(e)} />
                                <br />
                                {this.state.errors.fullname.length > 0 &&
                                    <span className='error'>{this.state.errors.fullname}</span>}
                            </li>
                            <li>
                                <input type="text" pattern="/^\d+$/ . $" name="npi" className="inputRegister" placeholder="NPI number" onChange={(e) => this.setValue(e)} />
                                <br />
                                {this.state.errors.npi.length > 0 &&
                                    <span className='error'>{this.state.errors.npi}</span>}
                            </li>
                            <li>
                                <input type="text" name="busaddress" className="inputRegister" placeholder="Business Address" onChange={(e) => this.setValue(e)} />
                                <br />
                                {this.state.errors.busaddress.length > 0 &&
                                    <span className='error'>{this.state.errors.busaddress}</span>}
                            </li>
                            <li>
                                    <PhoneInput country={'us'} specialLabel={''} className="inputRegister" name="phone" value={this.state.phone} onChange={phone => this.setState({ phone })} preventDefault />
                                {/* Removed react-phone-number-input because the version it requires is not compatiable with the hooks and version I am using for other newer react components*/}
                                <br />
                                {this.state.errors.phone.length > 0 &&
                                    <span className='error'>{this.state.errors.phone}</span>}                                
                            </li>
                            <li>
                                    <input type="email" name="email" className="inputRegister" placeholder="Email address (tcdwalker@availity.com)" onChange={(e) => this.setValue(e)} />
                                <br />
                                {this.state.errors.email.length > 0 &&
                                    <span className='error'>{this.state.errors.email}</span>}
                            </li>
                            <li>
                                <input type="password" name="password" className="inputRegister" placeholder="Password" onChange={(e) => this.setValue(e)} />
                                <br />
                                {this.state.errors.password.length > 0 &&
                                    <span className='error'>{this.state.errors.password}</span>}
                            </li>
                        </ul>
                        <img src={RegBtn} alt="Register" className="RegisterBtn" onClick={(e) => this.initSubmit(e)} />
                        <div className="footerBtns">
                            <p>Login with Email Account</p>
                            <MicrosoftLoginButton alt="Login with Microsoft" className="socialMediaBtn" onClick={() => alert("Hello Login with Microsoft Account")} />&nbsp;&nbsp;<GoogleLoginButton alt="Login with Google" className="socialMediaBtn" onClick={() => alert("Hello Login with Microsoft Account")} />
                        </div>
                    </div>
                    </div>
                    </React.StrictMode>
            </form>
        );
    }
}
