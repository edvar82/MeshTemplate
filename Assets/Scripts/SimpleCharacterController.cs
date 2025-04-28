using UnityEngine;

public class SimpleCharacterController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float gravity = 9.8f;

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        // Adicionar um CharacterController se n�o existir
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.3f;
            controller.slopeLimit = 45f;
            controller.stepOffset = 0.3f;
            Debug.Log("CharacterController adicionado � c�mera");
        }
    }

    void Update()
    {
        if (controller.isGrounded)
        {
            // Controle de movimento b�sico
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= moveSpeed;
        }

        // Aplicar gravidade
        moveDirection.y -= gravity * Time.deltaTime;

        // Mover o controller com detec��o de colis�o
        CollisionFlags flags = controller.Move(moveDirection * Time.deltaTime);

        // Verificar se houve colis�o
        if ((flags & CollisionFlags.CollidedSides) != 0)
        {
            Debug.Log("Colidiu com uma parede!");
        }
    }
}