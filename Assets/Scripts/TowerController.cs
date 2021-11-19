using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class TowerController : MonoBehaviour
{
    [SerializeField] private float maxJumpForce;
    [SerializeField] [Range(.1f, 1)] private float jumpTimeMultiplier;
    [SerializeField] private GameObject towerBlock;

    private List<GameObject> blocks;
    private float jumpForce;
    private Coroutine jumpForceCoroutine;
    private bool isGrounded;
    private Sequence jumpSequence;

    private void Awake()
    {
        blocks = new List<GameObject>();
        blocks.Add(GetComponentInChildren<TowerBlock>().gameObject);
        isGrounded = true;
    }

    private void Start()
    {
        jumpSequence = DOTween.Sequence();
    }

    private void OnEnable()
    {
        InputManager.Instance.OnStartTouch += GetJumpForce;
        InputManager.Instance.OnEndTouch += Jump;

    }
    private void OnDisable()
    {
        InputManager.Instance.OnStartTouch -= GetJumpForce;
        InputManager.Instance.OnEndTouch -= Jump;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Neutral"))
        {
            Destroy(other.gameObject);
            AddBlock();
        }
        else if (other.CompareTag("Enemy"))
        {
            RemoveBlock(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var reversedBlocks = new List<GameObject>(blocks);
            reversedBlocks.Reverse();

            for (int i = 0; i < reversedBlocks.Count; i++)
            {
                jumpSequence.Append(reversedBlocks[i].transform.DOLocalJump(Vector3.up * i, 0, 1, .5f, false).SetAutoKill());
            }
        }
    }

    private void GetJumpForce(Vector2 position, float time)
    {
        jumpForceCoroutine = StartCoroutine(IncreasingJumpForce());
    }

    private void Jump(Vector2 position, float time)
    {
        if (jumpForceCoroutine != null)
            StopCoroutine(jumpForceCoroutine);
        if (isGrounded)
        {
            var gravity = Physics.gravity.magnitude;
            var jumpHeight = jumpForce * jumpForce / (2 * gravity);
            var jumpDuration = 2 * jumpForce / gravity * jumpTimeMultiplier;

            jumpSequence.Append(transform.DOLocalJump(Vector3.zero, jumpHeight, 1, jumpDuration, false).SetEase(Ease.Linear)
                .OnStart(() => isGrounded = false)
                .OnComplete(() => isGrounded = true)
                .SetAutoKill());
        }
    }

    IEnumerator IncreasingJumpForce()
    {
        jumpForce = 0;

        for (int i = 0; jumpForce < maxJumpForce; i += 3)
        {
            jumpForce = maxJumpForce * i * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        jumpForce = maxJumpForce;
    }

    private void RemoveBlock(GameObject enemy)
    {
        var block = blocks.Last();
            block.GetComponent<TowerBlock>().SetNeutral();
            block.transform.parent = enemy.transform;
            block.transform.localPosition = -transform.parent.forward;
        
        blocks.Remove(block);

        if (!blocks.Any())
        {
            jumpSequence.Kill();
            GameManager.Instance.GameOver();
        }
    }

    private void AddBlock()
    {
        UpdateBlocksHeight();
    }

    private void UpdateBlocksHeight(int blockId = 0)
    {
        if (blockId < blocks.Count)
        {
            blocks[blockId].transform.localPosition += Vector3.up;
            UpdateBlocksHeight(blockId + 1);
        }
        else
        {
            var newBlock = Instantiate(towerBlock, transform);
                newBlock.transform.localPosition = Vector3.zero;

            blocks.Add(newBlock);
        }
    }
}
