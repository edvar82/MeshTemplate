using UnityEngine;

public class SimpleCharacterController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float gravity = 9.8f;

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        // Adicionar um CharacterController se não existir
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.3f;
            controller.slopeLimit = 45f;
            controller.stepOffset = 0.3f;
            Debug.Log("CharacterController adicionado à câmera");
        }
    }

    void Update()
    {
        if (controller.isGrounded)
        {
            // Controle de movimento básico
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= moveSpeed;
        }

        // Aplicar gravidade
        moveDirection.y -= gravity * Time.deltaTime;

        // Mover o controller com detecção de colisão
        CollisionFlags flags = controller.Move(moveDirection * Time.deltaTime);

        // Verificar se houve colisão
        if ((flags & CollisionFlags.CollidedSides) != 0)
        {
            Debug.Log("Colidiu com uma parede!");
        }
    }
}