using UnityEngine;

public class FootstepController : MonoBehaviour
{
    public float walkInterval = 0.5f;
    public float sprintInterval = 0.3f;

    private float timer = 0f;

    void Update()
    {
        // 直接看键盘输入，不看位置变化
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f;

        if (!isMoving)
        {
            timer = 0f;
            return;
        }

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float interval = isSprinting ? sprintInterval : walkInterval;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            AudioManager.instance?.PlayFootstep();
            timer = 0f;
        }
    }
}