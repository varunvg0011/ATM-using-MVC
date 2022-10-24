function IsValidNewEmailId(newEmailId) {
    var regexpattern = new RegExp("^[a-zA-Z0-9+_.-]$")
    if (newEmailId.match(regexpattern)) {
        return true;
    }
    return false;
}



function IsValidNewPhNo(newPhNo) {
    var regexpattern = new RegExp("^[1-90-9]{10}$")
    if (newPhNo.match(regexpattern)) {
        return true;
    }
    return false;
}

function isValidNewName(newName) {
    var regexpattern = new RegExp("^[a-zA-Z ]{1,40}$")
    if (newName.match(regexpattern)) {
        return true;
    }
    return false;
}

function IsValidNewDOB(newDOB) {
    var regexpattern = new RegExp("^[0-9/]{10}$")
    if (newDOB.match(regexpattern)) {
        return true;
    }
    return false;
}


function IsValidNewAddress(newAddress) {
    var regexpattern = new RegExp("^[,/.0-9a-zA-Z ]{1,100}$")
    if (newAddress.match(regexpattern)) {
        return true;
    }
    return false;
}


function IsValidAmount(amount) {
    var regexpattern = new RegExp("^[0-9]{1,60}$")
    if (amount.match(regexpattern)) {
        return true;
    }
    return false;
}