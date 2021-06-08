using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToastDisplay : MonoBehaviour {
	public static ToastDisplay Instance { get; private set; }

	[SerializeField] Text _titleText;
	[SerializeField] Text _descriptionText;
	[SerializeField] RectTransform _transformToMove;
	[SerializeField] RectTransform _inPosition;
	[SerializeField] RectTransform _outPosition;
	[SerializeField] float _transitionDuration = 0.5f;
	[SerializeField] float _holdDuration = 2.0f;

	public struct ToastMessage {
		public string Title;
		public string Description;
	}

	private Queue<ToastMessage> _queueToastMessages;
	private bool _isShowing = false;
	
	private void Awake () {
		if(Instance == null) {
			Instance = this;
			_queueToastMessages = new Queue<ToastMessage>();
		} else {
			Destroy(this.gameObject);
		}
	}

	public void Toast(string title, string description) {
		_queueToastMessages.Enqueue(new ToastMessage { Title = title, Description = description });

		if(!_isShowing) {
			StartCoroutine("ShowToast");
		}
	}

	private IEnumerator ShowToast() {
		_isShowing = true;
		ToastMessage toastToShow = _queueToastMessages.Dequeue();

		_titleText.text = toastToShow.Title;
		_descriptionText.text = toastToShow.Description;

		float distanceToMove = Vector3.Distance(_outPosition.anchoredPosition, _inPosition.anchoredPosition);

		// Start out
		_transformToMove.anchoredPosition = _outPosition.anchoredPosition;

		// Slide in
		while(Vector3.Distance(_transformToMove.anchoredPosition, _inPosition.anchoredPosition) > 0) {
			yield return null;
			_transformToMove.anchoredPosition = Vector3.MoveTowards(_transformToMove.anchoredPosition, _inPosition.anchoredPosition, distanceToMove*Time.deltaTime/_transitionDuration);
		}

		// Hold in
		yield return new WaitForSeconds(_holdDuration);

		// Slide out
		while(Vector3.Distance(_transformToMove.anchoredPosition, _outPosition.anchoredPosition) > 0) {
			yield return null;
			_transformToMove.anchoredPosition = Vector3.MoveTowards(_transformToMove.anchoredPosition, _outPosition.anchoredPosition, distanceToMove*Time.deltaTime/_transitionDuration);
		}

		_isShowing = false;

		if(_queueToastMessages.Count > 0) {
			StartCoroutine("ShowToast");
		}
	}
}
