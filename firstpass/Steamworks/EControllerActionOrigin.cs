namespace Steamworks
{
	public enum EControllerActionOrigin
	{
		k_EControllerActionOrigin_None,
		k_EControllerActionOrigin_A,
		k_EControllerActionOrigin_B,
		k_EControllerActionOrigin_X,
		k_EControllerActionOrigin_Y,
		k_EControllerActionOrigin_LeftBumper,
		k_EControllerActionOrigin_RightBumper,
		k_EControllerActionOrigin_LeftGrip,
		k_EControllerActionOrigin_RightGrip,
		k_EControllerActionOrigin_Start,
		k_EControllerActionOrigin_Back,
		k_EControllerActionOrigin_LeftPad_Touch,
		k_EControllerActionOrigin_LeftPad_Swipe,
		k_EControllerActionOrigin_LeftPad_Click,
		k_EControllerActionOrigin_LeftPad_DPadNorth,
		k_EControllerActionOrigin_LeftPad_DPadSouth,
		k_EControllerActionOrigin_LeftPad_DPadWest,
		k_EControllerActionOrigin_LeftPad_DPadEast,
		k_EControllerActionOrigin_RightPad_Touch,
		k_EControllerActionOrigin_RightPad_Swipe,
		k_EControllerActionOrigin_RightPad_Click,
		k_EControllerActionOrigin_RightPad_DPadNorth,
		k_EControllerActionOrigin_RightPad_DPadSouth,
		k_EControllerActionOrigin_RightPad_DPadWest,
		k_EControllerActionOrigin_RightPad_DPadEast,
		k_EControllerActionOrigin_LeftTrigger_Pull,
		k_EControllerActionOrigin_LeftTrigger_Click,
		k_EControllerActionOrigin_RightTrigger_Pull,
		k_EControllerActionOrigin_RightTrigger_Click,
		k_EControllerActionOrigin_LeftStick_Move,
		k_EControllerActionOrigin_LeftStick_Click,
		k_EControllerActionOrigin_LeftStick_DPadNorth,
		k_EControllerActionOrigin_LeftStick_DPadSouth,
		k_EControllerActionOrigin_LeftStick_DPadWest,
		k_EControllerActionOrigin_LeftStick_DPadEast,
		k_EControllerActionOrigin_Gyro_Move,
		k_EControllerActionOrigin_Gyro_Pitch,
		k_EControllerActionOrigin_Gyro_Yaw,
		k_EControllerActionOrigin_Gyro_Roll,
		k_EControllerActionOrigin_PS4_X,
		k_EControllerActionOrigin_PS4_Circle,
		k_EControllerActionOrigin_PS4_Triangle,
		k_EControllerActionOrigin_PS4_Square,
		k_EControllerActionOrigin_PS4_LeftBumper,
		k_EControllerActionOrigin_PS4_RightBumper,
		k_EControllerActionOrigin_PS4_Options,
		k_EControllerActionOrigin_PS4_Share,
		k_EControllerActionOrigin_PS4_LeftPad_Touch,
		k_EControllerActionOrigin_PS4_LeftPad_Swipe,
		k_EControllerActionOrigin_PS4_LeftPad_Click,
		k_EControllerActionOrigin_PS4_LeftPad_DPadNorth,
		k_EControllerActionOrigin_PS4_LeftPad_DPadSouth,
		k_EControllerActionOrigin_PS4_LeftPad_DPadWest,
		k_EControllerActionOrigin_PS4_LeftPad_DPadEast,
		k_EControllerActionOrigin_PS4_RightPad_Touch,
		k_EControllerActionOrigin_PS4_RightPad_Swipe,
		k_EControllerActionOrigin_PS4_RightPad_Click,
		k_EControllerActionOrigin_PS4_RightPad_DPadNorth,
		k_EControllerActionOrigin_PS4_RightPad_DPadSouth,
		k_EControllerActionOrigin_PS4_RightPad_DPadWest,
		k_EControllerActionOrigin_PS4_RightPad_DPadEast,
		k_EControllerActionOrigin_PS4_CenterPad_Touch,
		k_EControllerActionOrigin_PS4_CenterPad_Swipe,
		k_EControllerActionOrigin_PS4_CenterPad_Click,
		k_EControllerActionOrigin_PS4_CenterPad_DPadNorth,
		k_EControllerActionOrigin_PS4_CenterPad_DPadSouth,
		k_EControllerActionOrigin_PS4_CenterPad_DPadWest,
		k_EControllerActionOrigin_PS4_CenterPad_DPadEast,
		k_EControllerActionOrigin_PS4_LeftTrigger_Pull,
		k_EControllerActionOrigin_PS4_LeftTrigger_Click,
		k_EControllerActionOrigin_PS4_RightTrigger_Pull,
		k_EControllerActionOrigin_PS4_RightTrigger_Click,
		k_EControllerActionOrigin_PS4_LeftStick_Move,
		k_EControllerActionOrigin_PS4_LeftStick_Click,
		k_EControllerActionOrigin_PS4_LeftStick_DPadNorth,
		k_EControllerActionOrigin_PS4_LeftStick_DPadSouth,
		k_EControllerActionOrigin_PS4_LeftStick_DPadWest,
		k_EControllerActionOrigin_PS4_LeftStick_DPadEast,
		k_EControllerActionOrigin_PS4_RightStick_Move,
		k_EControllerActionOrigin_PS4_RightStick_Click,
		k_EControllerActionOrigin_PS4_RightStick_DPadNorth,
		k_EControllerActionOrigin_PS4_RightStick_DPadSouth,
		k_EControllerActionOrigin_PS4_RightStick_DPadWest,
		k_EControllerActionOrigin_PS4_RightStick_DPadEast,
		k_EControllerActionOrigin_PS4_DPad_North,
		k_EControllerActionOrigin_PS4_DPad_South,
		k_EControllerActionOrigin_PS4_DPad_West,
		k_EControllerActionOrigin_PS4_DPad_East,
		k_EControllerActionOrigin_PS4_Gyro_Move,
		k_EControllerActionOrigin_PS4_Gyro_Pitch,
		k_EControllerActionOrigin_PS4_Gyro_Yaw,
		k_EControllerActionOrigin_PS4_Gyro_Roll,
		k_EControllerActionOrigin_XBoxOne_A,
		k_EControllerActionOrigin_XBoxOne_B,
		k_EControllerActionOrigin_XBoxOne_X,
		k_EControllerActionOrigin_XBoxOne_Y,
		k_EControllerActionOrigin_XBoxOne_LeftBumper,
		k_EControllerActionOrigin_XBoxOne_RightBumper,
		k_EControllerActionOrigin_XBoxOne_Menu,
		k_EControllerActionOrigin_XBoxOne_View,
		k_EControllerActionOrigin_XBoxOne_LeftTrigger_Pull,
		k_EControllerActionOrigin_XBoxOne_LeftTrigger_Click,
		k_EControllerActionOrigin_XBoxOne_RightTrigger_Pull,
		k_EControllerActionOrigin_XBoxOne_RightTrigger_Click,
		k_EControllerActionOrigin_XBoxOne_LeftStick_Move,
		k_EControllerActionOrigin_XBoxOne_LeftStick_Click,
		k_EControllerActionOrigin_XBoxOne_LeftStick_DPadNorth,
		k_EControllerActionOrigin_XBoxOne_LeftStick_DPadSouth,
		k_EControllerActionOrigin_XBoxOne_LeftStick_DPadWest,
		k_EControllerActionOrigin_XBoxOne_LeftStick_DPadEast,
		k_EControllerActionOrigin_XBoxOne_RightStick_Move,
		k_EControllerActionOrigin_XBoxOne_RightStick_Click,
		k_EControllerActionOrigin_XBoxOne_RightStick_DPadNorth,
		k_EControllerActionOrigin_XBoxOne_RightStick_DPadSouth,
		k_EControllerActionOrigin_XBoxOne_RightStick_DPadWest,
		k_EControllerActionOrigin_XBoxOne_RightStick_DPadEast,
		k_EControllerActionOrigin_XBoxOne_DPad_North,
		k_EControllerActionOrigin_XBoxOne_DPad_South,
		k_EControllerActionOrigin_XBoxOne_DPad_West,
		k_EControllerActionOrigin_XBoxOne_DPad_East,
		k_EControllerActionOrigin_XBox360_A,
		k_EControllerActionOrigin_XBox360_B,
		k_EControllerActionOrigin_XBox360_X,
		k_EControllerActionOrigin_XBox360_Y,
		k_EControllerActionOrigin_XBox360_LeftBumper,
		k_EControllerActionOrigin_XBox360_RightBumper,
		k_EControllerActionOrigin_XBox360_Start,
		k_EControllerActionOrigin_XBox360_Back,
		k_EControllerActionOrigin_XBox360_LeftTrigger_Pull,
		k_EControllerActionOrigin_XBox360_LeftTrigger_Click,
		k_EControllerActionOrigin_XBox360_RightTrigger_Pull,
		k_EControllerActionOrigin_XBox360_RightTrigger_Click,
		k_EControllerActionOrigin_XBox360_LeftStick_Move,
		k_EControllerActionOrigin_XBox360_LeftStick_Click,
		k_EControllerActionOrigin_XBox360_LeftStick_DPadNorth,
		k_EControllerActionOrigin_XBox360_LeftStick_DPadSouth,
		k_EControllerActionOrigin_XBox360_LeftStick_DPadWest,
		k_EControllerActionOrigin_XBox360_LeftStick_DPadEast,
		k_EControllerActionOrigin_XBox360_RightStick_Move,
		k_EControllerActionOrigin_XBox360_RightStick_Click,
		k_EControllerActionOrigin_XBox360_RightStick_DPadNorth,
		k_EControllerActionOrigin_XBox360_RightStick_DPadSouth,
		k_EControllerActionOrigin_XBox360_RightStick_DPadWest,
		k_EControllerActionOrigin_XBox360_RightStick_DPadEast,
		k_EControllerActionOrigin_XBox360_DPad_North,
		k_EControllerActionOrigin_XBox360_DPad_South,
		k_EControllerActionOrigin_XBox360_DPad_West,
		k_EControllerActionOrigin_XBox360_DPad_East,
		k_EControllerActionOrigin_SteamV2_A,
		k_EControllerActionOrigin_SteamV2_B,
		k_EControllerActionOrigin_SteamV2_X,
		k_EControllerActionOrigin_SteamV2_Y,
		k_EControllerActionOrigin_SteamV2_LeftBumper,
		k_EControllerActionOrigin_SteamV2_RightBumper,
		k_EControllerActionOrigin_SteamV2_LeftGrip,
		k_EControllerActionOrigin_SteamV2_RightGrip,
		k_EControllerActionOrigin_SteamV2_LeftGrip_Upper,
		k_EControllerActionOrigin_SteamV2_RightGrip_Upper,
		k_EControllerActionOrigin_SteamV2_LeftBumper_Pressure,
		k_EControllerActionOrigin_SteamV2_RightBumper_Pressure,
		k_EControllerActionOrigin_SteamV2_LeftGrip_Pressure,
		k_EControllerActionOrigin_SteamV2_RightGrip_Pressure,
		k_EControllerActionOrigin_SteamV2_LeftGrip_Upper_Pressure,
		k_EControllerActionOrigin_SteamV2_RightGrip_Upper_Pressure,
		k_EControllerActionOrigin_SteamV2_Start,
		k_EControllerActionOrigin_SteamV2_Back,
		k_EControllerActionOrigin_SteamV2_LeftPad_Touch,
		k_EControllerActionOrigin_SteamV2_LeftPad_Swipe,
		k_EControllerActionOrigin_SteamV2_LeftPad_Click,
		k_EControllerActionOrigin_SteamV2_LeftPad_Pressure,
		k_EControllerActionOrigin_SteamV2_LeftPad_DPadNorth,
		k_EControllerActionOrigin_SteamV2_LeftPad_DPadSouth,
		k_EControllerActionOrigin_SteamV2_LeftPad_DPadWest,
		k_EControllerActionOrigin_SteamV2_LeftPad_DPadEast,
		k_EControllerActionOrigin_SteamV2_RightPad_Touch,
		k_EControllerActionOrigin_SteamV2_RightPad_Swipe,
		k_EControllerActionOrigin_SteamV2_RightPad_Click,
		k_EControllerActionOrigin_SteamV2_RightPad_Pressure,
		k_EControllerActionOrigin_SteamV2_RightPad_DPadNorth,
		k_EControllerActionOrigin_SteamV2_RightPad_DPadSouth,
		k_EControllerActionOrigin_SteamV2_RightPad_DPadWest,
		k_EControllerActionOrigin_SteamV2_RightPad_DPadEast,
		k_EControllerActionOrigin_SteamV2_LeftTrigger_Pull,
		k_EControllerActionOrigin_SteamV2_LeftTrigger_Click,
		k_EControllerActionOrigin_SteamV2_RightTrigger_Pull,
		k_EControllerActionOrigin_SteamV2_RightTrigger_Click,
		k_EControllerActionOrigin_SteamV2_LeftStick_Move,
		k_EControllerActionOrigin_SteamV2_LeftStick_Click,
		k_EControllerActionOrigin_SteamV2_LeftStick_DPadNorth,
		k_EControllerActionOrigin_SteamV2_LeftStick_DPadSouth,
		k_EControllerActionOrigin_SteamV2_LeftStick_DPadWest,
		k_EControllerActionOrigin_SteamV2_LeftStick_DPadEast,
		k_EControllerActionOrigin_SteamV2_Gyro_Move,
		k_EControllerActionOrigin_SteamV2_Gyro_Pitch,
		k_EControllerActionOrigin_SteamV2_Gyro_Yaw,
		k_EControllerActionOrigin_SteamV2_Gyro_Roll,
		k_EControllerActionOrigin_Count
	}
}
