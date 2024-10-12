using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class ItemThrower : MonoBehaviour
{
    [SerializeField] private float _throwForce;
    [SerializeField] private float _throwVerticalForce;
    private GameObject _heldObject;
    private Transform _oldParent;
    private float _pickupTick;

    private InputAction _throwAction;
    private InputAction _interactAction;

    private void Start()
    {
        _throwAction = InputSystem.actions.FindAction("Attack");
        _interactAction = InputSystem.actions.FindAction("Interact");
    }

    private void Update()
    {
        if(CanThrow() && _throwAction.WasPressedThisFrame())
        {
            Throw();
        }
        if(CanThrow() && _interactAction.WasPressedThisFrame() && Time.time - _pickupTick >= 0.1f)
        {
            DropObject();
        }
    }

    void Throw()
    {
        Rigidbody _rb = _heldObject.GetComponent<Rigidbody>();
        _rb.isKinematic = false;
        _rb.AddForce(Camera.main.transform.forward * _throwForce + Vector3.up * _throwVerticalForce, ForceMode.Impulse);
        _heldObject.transform.parent = _oldParent;
        _heldObject = null;
    }

    public void DropObject()
    {
        Rigidbody _rb = _heldObject.GetComponent<Rigidbody>();
        _rb.isKinematic = false;
        _heldObject.transform.parent = _oldParent;
        _heldObject = null;
    }

    public void GrabObject(GameObject _obj)
    {
        if(_heldObject != null)
        {
            DropObject();
        }
        Vector3 _offset = Vector3.zero;
        Quaternion _rotation = Quaternion.identity;
        if(_obj.transform.Find("GrabOffset") != null)
        {
            _offset = _obj.transform.Find("GrabOffset").position - _obj.transform.position;
            _rotation = _obj.transform.Find("GrabOffset").rotation;
        }
        _oldParent = _obj.transform.parent;
        _obj.transform.position = transform.position + _offset;
        _obj.transform.rotation = _rotation;
        _obj.transform.parent = transform;
        if (_obj.GetComponent<Rigidbody>() != null)
        {
            Rigidbody _rb = _obj.GetComponent<Rigidbody>();
            _rb.isKinematic = true;
        }
        _heldObject = _obj;
        _pickupTick = Time.time;
    }
    bool CanThrow()
    {
        return _heldObject != null;
    }
}
