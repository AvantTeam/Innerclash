using UnityEngine;

public class PlayerController : EntityController {
    private void Update() {
        float inputX = Input.GetAxis("Horizontal");
        controllable.Move(inputX);
    }
}
