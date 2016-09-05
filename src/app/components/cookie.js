export default function Cookie(container) {
    const agreeSlug = 'privacy_policy';

    const agreeBtn = container.querySelector('.agree');
    const hasAgreed = agreedBefore();
    if (!hasAgreed) {
        container.classList.add('privacy-policy-visible');
    }

    /* Handle click */
    agreeBtn.addEventListener('click', (e) => {
        e.preventDefault();

        /* Write to the cookies */
        document.cookie = agreeSlug + '=1; expires=Fri, 31 Dec 9999 23:59:59 GMT; path=/';
        container.classList.remove('privacy-policy-visible');
    });

    /* Check if the customer has previously agreed to Privacy Policy */
    function agreedBefore() {
        const cookies = document.cookie.split('; ');
        const hasAgreed = cookies.filter(entry => entry.includes(agreeSlug))[0];
        return !!hasAgreed;
    }
}