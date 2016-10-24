import 'babel-polyfill';
import 'svgxuse';
import init from './init';
import factory from './factory';
import Cookie from './components/cookie';
import FormAuth from './components/forms/authentication';
import DateInput from './components/datepicker';
import FormOffer from './components/forms/offer';
import CheckAll from './components/check-all';
import Copyright from './components/copyright';

window.app = {
    start(config) {
        init(Cookie, document.body);
        init(FormAuth, document.getElementById('authentication'));
        init(FormOffer, document.getElementById('offer'));
        factory(CheckAll, $('.form .check-all'));
        init(DateInput, document.querySelector('.input-date'));
        init(Copyright, document.getElementById('copyright'));
    }
};
