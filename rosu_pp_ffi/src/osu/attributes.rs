use interoptopus::ffi_type;

/// The result of a difficulty calculation on an osu!standard map.
#[derive(Clone, Debug, Default, PartialEq)]
#[repr(C)]
#[ffi_type]
pub struct OsuDifficultyAttributes {
    /// The difficulty of the aim skill.
    pub aim: f64,
    /// The number of sliders weighted by difficulty.
    pub aim_difficult_slider_count: f64,
    /// The difficulty of the jump skill.
    pub jump: f64,
    /// The difficulty of the flow skill.
    pub flow: f64,
    /// The difficulty of the precision skill.
    pub precision: f64,
    /// The difficulty of the speed skill.
    pub speed: f64,
    /// The difficulty of the stamina skill.
    pub stamina: f64,
    /// The difficulty of the accuracy skill.
    pub accuracy: f64,
    /// The ratio of the aim strain with and without considering sliders
    pub slider_factor: f64,
    /// The number of clickable objects weighted by difficulty.
    pub speed_note_count: f64,
    /// Weighted sum of aim strains.
    pub aim_difficult_strain_count: f64,
    /// Weighted sum of jump aim strains.
    pub jump_aim_difficult_strain_count: f64,
    /// Weighted sum of flow aim strains.
    pub flow_aim_difficult_strain_count: f64,
    /// Weighted sum of speed strains.
    pub speed_difficult_strain_count: f64,
    /// Weighted sum of stamina strains.
    pub stamina_difficult_strain_count: f64,
    /// The approach rate.
    pub ar: f64,
    /// The great hit window.
    pub great_hit_window: f64,
    /// The ok hit window.
    pub ok_hit_window: f64,
    /// The meh hit window.
    pub meh_hit_window: f64,
    /// The health drain rate.
    pub hp: f64,
    /// The amount of circles.
    pub n_circles: u32,
    /// The amount of sliders.
    pub n_sliders: u32,
    /// The amount of "large ticks".
    ///
    /// The meaning depends on the kind of score:
    /// - if set on osu!stable, this value is irrelevant
    /// - if set on osu!lazer *with* slider accuracy, this value is the amount
    ///   of hit slider ticks and repeats
    /// - if set on osu!lazer *without* slider accuracy, this value is the
    ///   amount of hit slider heads, ticks, and repeats
    pub n_large_ticks: u32,
    /// The amount of spinners.
    pub n_spinners: u32,
    /// The final star rating
    pub stars: f64,
    /// The maximum combo.
    pub max_combo: u32,
}

impl OsuDifficultyAttributes {
    /// Return the maximum combo.
    pub const fn max_combo(&self) -> u32 {
        self.max_combo
    }

    /// Return the amount of hitobjects.
    pub const fn n_objects(&self) -> u32 {
        self.n_circles + self.n_sliders + self.n_spinners
    }
}

impl From<rosu_pp::osu::OsuDifficultyAttributes> for OsuDifficultyAttributes {
    fn from(attributes: rosu_pp::osu::OsuDifficultyAttributes) -> Self {
        Self {
            aim: attributes.aim,
            aim_difficult_slider_count: attributes.aim_difficult_slider_count,
            jump: attributes.jump,
            flow: attributes.flow,
            precision: attributes.precision,
            speed: attributes.speed,
            stamina: attributes.stamina,
            accuracy: attributes.accuracy,
            slider_factor: attributes.slider_factor,
            speed_note_count: attributes.speed_note_count,
            aim_difficult_strain_count: attributes.aim_difficult_strain_count,
            jump_aim_difficult_strain_count: attributes.jump_aim_difficult_strain_count,
            flow_aim_difficult_strain_count: attributes.flow_aim_difficult_strain_count,
            speed_difficult_strain_count: attributes.speed_difficult_strain_count,
            stamina_difficult_strain_count: attributes.stamina_difficult_strain_count,
            ar: attributes.ar,
            great_hit_window: attributes.great_hit_window,
            ok_hit_window: attributes.ok_hit_window,
            meh_hit_window: attributes.meh_hit_window,
            hp: attributes.hp,
            n_circles: attributes.n_circles,
            n_sliders: attributes.n_sliders,
            n_large_ticks: attributes.n_large_ticks,
            n_spinners: attributes.n_spinners,
            stars: attributes.stars,
            max_combo: attributes.max_combo,
        }
    }
}

impl From<OsuDifficultyAttributes> for rosu_pp::osu::OsuDifficultyAttributes {
    fn from(attributes: OsuDifficultyAttributes) -> Self {
        Self {
            aim: attributes.aim,
            aim_difficult_slider_count: attributes.aim_difficult_slider_count,
            jump: attributes.jump,
            flow: attributes.flow,
            precision: attributes.precision,
            speed: attributes.speed,
            stamina: attributes.stamina,
            accuracy: attributes.accuracy,
            slider_factor: attributes.slider_factor,
            speed_note_count: attributes.speed_note_count,
            aim_difficult_strain_count: attributes.aim_difficult_strain_count,
            jump_aim_difficult_strain_count: attributes.jump_aim_difficult_strain_count,
            flow_aim_difficult_strain_count: attributes.flow_aim_difficult_strain_count,
            speed_difficult_strain_count: attributes.speed_difficult_strain_count,
            stamina_difficult_strain_count: attributes.stamina_difficult_strain_count,
            ar: attributes.ar,
            great_hit_window: attributes.great_hit_window,
            ok_hit_window: attributes.ok_hit_window,
            meh_hit_window: attributes.meh_hit_window,
            hp: attributes.hp,
            n_circles: attributes.n_circles,
            n_sliders: attributes.n_sliders,
            n_large_ticks: attributes.n_large_ticks,
            n_spinners: attributes.n_spinners,
            stars: attributes.stars,
            max_combo: attributes.max_combo,
        }
    }
}

/// The result of a performance calculation on an osu!standard map.
#[derive(Clone, Debug, Default, PartialEq)]
#[repr(C)]
#[ffi_type]
pub struct OsuPerformanceAttributes {
    /// The difficulty attributes that were used for the performance calculation
    pub difficulty: OsuDifficultyAttributes,
    /// The final performance points.
    pub pp: f64,
    /// The aim portion of the final pp.
    pub pp_aim: f64,
    /// The jump aim portion of the final pp.
    pub pp_jump_aim: f64,
    /// The flow aim portion of the final pp.
    pub pp_flow_aim: f64,
    /// The precision portion of the final pp.
    pub pp_precision: f64,
    /// The speed portion of the final pp.
    pub pp_speed: f64,
    /// The stamina portion of the final pp.
    pub pp_stamina: f64,
    /// The acc portion of the final pp.
    pub pp_acc: f64,
    /// Misses including an approximated amount of slider breaks
    pub effective_miss_count: f64,
}

impl OsuPerformanceAttributes {
    /// Return the star value.
    pub const fn stars(&self) -> f64 {
        self.difficulty.stars
    }

    /// Return the performance point value.
    pub const fn pp(&self) -> f64 {
        self.pp
    }

    /// Return the maximum combo of the map.
    pub const fn max_combo(&self) -> u32 {
        self.difficulty.max_combo
    }
    /// Return the amount of hitobjects.
    pub const fn n_objects(&self) -> u32 {
        self.difficulty.n_objects()
    }
}

impl From<rosu_pp::osu::OsuPerformanceAttributes> for OsuPerformanceAttributes {
    fn from(attributes: rosu_pp::osu::OsuPerformanceAttributes) -> Self {
        Self {
            difficulty: attributes.difficulty.into(),
            pp: attributes.pp,
            pp_aim: attributes.pp_aim,
            pp_jump_aim: attributes.pp_jump_aim,
            pp_flow_aim: attributes.pp_flow_aim,
            pp_precision: attributes.pp_precision,
            pp_speed: attributes.pp_speed,
            pp_stamina: attributes.pp_stamina,
            pp_acc: attributes.pp_acc,
            effective_miss_count: attributes.effective_miss_count,
        }
    }
}

impl From<OsuPerformanceAttributes> for rosu_pp::osu::OsuPerformanceAttributes {
    fn from(attributes: OsuPerformanceAttributes) -> Self {
        Self {
            difficulty: attributes.difficulty.into(),
            pp: attributes.pp,
            pp_aim: attributes.pp_aim,
            pp_jump_aim: attributes.pp_jump_aim,
            pp_flow_aim: attributes.pp_flow_aim,
            pp_precision: attributes.pp_precision,
            pp_speed: attributes.pp_speed,
            pp_stamina: attributes.pp_stamina,
            pp_acc: attributes.pp_acc,
            effective_miss_count: attributes.effective_miss_count,
        }
    }
}
