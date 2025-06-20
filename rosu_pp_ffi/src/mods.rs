use crate::*;
use interoptopus::{
    ffi_service, ffi_service_ctor, ffi_service_method, ffi_type,
    patterns::{option::FFIOption, string::AsciiPointer},
};
use mode::Mode;
use owned_string::OwnedString;
use rosu_mods::{generated_mods::UnknownMod, serde::{GameModSeed, GameModsSeed}, Acronym, GameMod};
use serde::de::DeserializeSeed;

#[ffi_type(opaque)]
#[derive(Default)]
pub struct Mods {
    pub mods: rosu_mods::GameMods,
    pub mode: Option<Mode>,
}

// Regular implementation of methods.
#[ffi_service(error = "FFIError", prefix = "mods_")]
impl Mods {
    #[ffi_service_ctor]
    pub fn new(mode: Mode) -> Result<Self, Error> {
        Ok(Self {
            mods: rosu_mods::GameMods::new(),
            mode: Some(mode)
        })
    }

    #[ffi_service_ctor]
    pub fn from_acronyms(str: AsciiPointer, mode: Mode) -> Result<Self, Error> {
        Ok(Self {
            mods: rosu_mods::GameMods::from_intermode(&rosu_mods::GameModsIntermode::from_acronyms(
                str.as_str()?,
            ), mode.into()),
            mode: Some(mode)
        })
    }

    #[ffi_service_ctor]
    pub fn from_bits(bits: u32, mode: Mode) -> Result<Self, Error> {
        Ok(Self {
            mods: rosu_mods::GameMods::from_intermode(&rosu_mods::GameModsIntermode::from_bits(bits), mode.into()),
            mode: Some(mode)
        })
    }


    #[ffi_service_ctor]
    pub fn from_json(str: AsciiPointer, mode: Mode, deny_unknown_fields: bool) -> Result<Self, Error> {
        let s = str.as_str()?;
        let mut d = serde_json::Deserializer::from_str(s);
        let mods = GameModsSeed::Mode { mode: mode.into(), deny_unknown_fields }.deserialize(&mut d)?;
        Ok(Self {
            mods,
            mode: Some(mode)
        })
    }
    
    #[ffi_service_method(on_panic = "undefined_behavior")]
    pub fn remove_unknown_mods(&mut self) {
        self.mods = self.mods.clone().into_iter().filter(|m| m.kind() != UnknownMod::kind()).collect();
    }
    

    #[ffi_service_method(on_panic = "undefined_behavior")]
    pub fn sanitize(&mut self) {
        self.mods.sanitize();
    }

    #[ffi_service_method(on_panic = "undefined_behavior")]
    pub fn bits(&mut self) -> u32 {
        self.mods.bits()
    }

    #[ffi_service_method(on_panic = "undefined_behavior")]
    pub fn len(&mut self) -> u32 {
        self.mods.len() as u32
    }

    #[ffi_service_method(on_panic = "undefined_behavior")]
    pub fn json(&mut self, str: &mut OwnedString) {
        str.replace(serde_json::to_string_pretty(&self.mods).unwrap())
    }

    #[ffi_service_method(on_panic = "undefined_behavior")]
    pub fn insert_json(&mut self, str: AsciiPointer, deny_unknown_fields: bool) -> bool {
        if let Ok(s) = str.as_str() {
            let mut d = serde_json::Deserializer::from_str(s);
            let s = if let Some(mode) = self.mode { GameModSeed::Mode { mode: mode.into(), deny_unknown_fields } } else { GameModSeed::GuessMode { deny_unknown_fields } };
            if let Ok(m) = s.deserialize(&mut d) {
                self.mods.insert(m);
                return true;
            }
        }
        false
    }

    #[ffi_service_method(on_panic = "undefined_behavior")]
    pub fn insert(&mut self, str: AsciiPointer) -> bool {
        if let Ok(s) = str.as_str() {
            self.mods.insert(GameMod::new(s, self.mode.unwrap_or_default().into()));
            return true;
        }
        false
    }

    #[ffi_service_method(on_panic = "undefined_behavior")]
    pub fn contains(&mut self, str: AsciiPointer) -> bool {
        if let Ok(s) = str.as_str() {
            if let Ok(m) = s.parse::<Acronym>(){
                return self.mods.contains_acronym(m);
            }
        }
        false
    }

    #[ffi_service_method(on_panic = "undefined_behavior")]
    pub fn clear(&mut self) {
        self.mods = rosu_mods::GameMods::new();
    }

    #[ffi_service_method(on_panic = "undefined_behavior")]
    pub fn clock_rate(&mut self) -> FFIOption<f64> {
        self.mods.clock_rate().into()
    }
}
