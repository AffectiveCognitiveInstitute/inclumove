namespace Aci.Unity.Models
{
    [System.Serializable]
    public struct Position
    {
        public float X;
        public float Y;
    }

    [System.Serializable]
    public struct PCB_Position
    {
        public float X;
        public float Y;
        public float PSI;
    }

    [System.Serializable]
    public struct Status
    {
        public uint state;
        public string error;
    }

    [System.Serializable]
    public struct Assembly_order_req
    {
        public uint req_id;
        public int order_id;
        public Position[] reference_marks;
    }

    [System.Serializable]
    public struct Assembly_order_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Assembly_order_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Insert_board_req
    {
        public uint req_id;
    }

    [System.Serializable]
    public struct Insert_board_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Insert_board_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Detect_reference_mark_req
    {
        public uint req_id;
    }

    [System.Serializable]
    public struct Detect_reference_mark_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Detect_reference_mark_res
    {
        public uint req_id;
        public bool result;
        public Position delta_position;
        public string error;
    }

    [System.Serializable]
    public struct Heading_angle_psi_req
    {
        public uint req_id;
        public float psi;
    }

    [System.Serializable]
    public struct Heading_angle_psi_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Heading_angle_psi_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Assembly_step_req
    {
        public uint req_id;
        public uint step_id;
        public int part_id;
        public int gripper_id;
        public PCB_Position pcb_position;
        public PCB_Position part_position;
        public float part_height;
        public string assembly_type;
    }

    [System.Serializable]
    public struct Assembly_step_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Assembly_step_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Detect_part_req
    {
        public uint req_id;
        public uint step_id;
        public int part_id;
    }

    [System.Serializable]
    public struct Detect_part_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Detect_part_res
    {
        public uint req_id;
        public uint step_id;
        public bool result;
        public PCB_Position delta_position;
        public string error;
    }

    [System.Serializable]
    public struct Detect_assembly_position_req
    {
        public uint req_id;
        public uint step_id;
        public int part_id;
        public PCB_Position part_position;
    }

    [System.Serializable]
    public struct Detect_assembly_position_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Detect_assembly_position_res
    {
        public uint req_id;
        public uint step_id;
        public bool result;
        public PCB_Position delta_position;
        public string error;
    }

    [System.Serializable]
    public struct Assembly_part_req
    {
        public uint req_id;
        public uint step_id;
    }

    [System.Serializable]
    public struct Assembly_part_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Assembly_part_res
    {
        public uint req_id;
        public PCB_Position delta_position;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Detect_gripper_req
    {
        public uint req_id;
        public uint step_id;
        public int gripper_id;
        public PCB_Position part_position;
    }

    [System.Serializable]
    public struct Detect_gripper_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Detect_gripper_res
    {
        public uint req_id;
        public uint step_id;
        public bool result;
        public PCB_Position delta_position;
        public string error;
    }

    [System.Serializable]
    public struct Gripper_offset_req
    {
        public uint req_id;
        public uint step_id;
        public bool result;
        public Position delta_position;
        public string error;
    }

    [System.Serializable]
    public struct Gripper_offset_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Gripper_offset_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct In_position_req
    {
        public uint req_id;
        public uint step_id;
        public bool result;
    }

    [System.Serializable]
    public struct In_position_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct In_position_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Assembly_done_req
    {
        public uint req_id;
        public uint step_id;
        public bool result;
    }

    [System.Serializable]
    public struct Assembly_done_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Assembly_done_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Inspect_single_part_req
    {
        public uint req_id;
        public Position part_position;
        public uint step_id;
        public int part_id;
    }

    [System.Serializable]
    public struct Inspect_single_part_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Inspect_single_part_res
    {
        public uint req_id;
        public uint step_id;
        public Position delta_position;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Assembly_end_req
    {
        public uint req_id;
    }

    [System.Serializable]
    public struct Assembly_end_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Assembly_end_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Inspect_assembly_req
    {
        public uint req_id;
        public uint step_id;
    }

    [System.Serializable]
    public struct Inspect_assembly_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Inspect_assembly_res
    {
        public uint req_id;
        public uint step_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Inspection_loop_req
    {
        public uint req_id;
        public bool enable;
    }

    [System.Serializable]
    public struct Inspection_loop_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Inspection_loop_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Pressured_air_req
    {
        public uint req_id;
        public bool enable;
    }

    [System.Serializable]
    public struct Pressured_air_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Pressured_air_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Reset_req
    {
        public uint req_id;
    }

    [System.Serializable]
    public struct Reset_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Reset_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Guide_gripper_drop_off_req
    {
        public uint req_id;
    }

    [System.Serializable]
    public struct Guide_gripper_drop_off_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Guide_gripper_drop_off_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Guide_gripper_pick_up_req
    {
        public uint req_id;
    }

    [System.Serializable]
    public struct Guide_gripper_pick_up_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Guide_gripper_pick_up_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Gripper_ready_req
    {
        public uint req_id;
    }

    [System.Serializable]
    public struct Gripper_ready_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Gripper_ready_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }

    [System.Serializable]
    public struct Part_picked_req
    {
        public uint req_id;
    }

    [System.Serializable]
    public struct Part_picked_ack
    {
        public uint req_id;
        public bool ack;
        public string error;
    }

    [System.Serializable]
    public struct Part_picked_res
    {
        public uint req_id;
        public bool result;
        public string error;
    }
}
