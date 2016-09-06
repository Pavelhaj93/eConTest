export default function Cookie(container) {

    /* Slug to look in the Cookies to determine if already agreed to Privacy Policy */
    const agreeSlug = 'privacy_policy';
    const hasAgreed = agreedBefore();

    const agreeBtn = container.querySelector('.agree');
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