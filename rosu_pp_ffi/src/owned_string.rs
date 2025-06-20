use std::{ffi::CString, mem::MaybeUninit};

use crate::*;
use interoptopus::{
    ffi_service, ffi_service_ctor, ffi_service_method, ffi_type,
    patterns::string::AsciiPointer,
};

#[ffi_type(opaque)]
pub struct OwnedString {
    pub inner: MaybeUninit<CString>,
    pub is_init: bool
}

// Regular implementation of methods.
#[ffi_service(error = "FFIError", prefix = "string_")]
impl OwnedString {
    #[ffi_service_ctor]
    pub fn from_c_str(str: AsciiPointer<'_>) -> Result<Self, Error> {
        Ok(Self {
            inner: MaybeUninit::new(str.as_c_str().ok_or(Error::Null)?.to_owned()),
            is_init: true
        })
    }

    #[ffi_service_ctor]
    pub fn empty() -> Result<Self, Error> {
        Ok(Self {
            inner: MaybeUninit::uninit(),
            is_init: false
        })
    }

    #[ffi_service_method(on_panic = "undefined_behavior")]
    pub fn is_init(&self) -> bool {
       self.is_init
    }

    #[ffi_service_method(on_panic = "undefined_behavior")]
    pub fn to_cstr(&self) -> AsciiPointer<'_> {
        AsciiPointer::from_cstr(unsafe { self.inner.assume_init_ref() })
    }
}

impl OwnedString {
    pub fn replace(&mut self, str: String) {
        if self.is_init { unsafe { self.inner.assume_init_drop(); } }
        self.inner.write(CString::new(str).unwrap());
        self.is_init = true;
    }
}

impl Drop for OwnedString {
    fn drop(&mut self) {
        if self.is_init { unsafe { self.inner.assume_init_drop(); } }
    }
}