#define DEBUG_CC2D_RAYS
using UnityEngine;
using System;
using System.Collections.Generic;


namespace Prime31 {

[RequireComponent( typeof( BoxCollider2D ), typeof( Rigidbody2D ) )]
public class CharacterController2D : MonoBehaviour
{
	#region internal types

	struct CharacterRaycastOrigins
	{
		public Vector3 topLeft;
		public Vector3 bottomRight;
		public Vector3 bottomLeft;
	}

	public class CharacterCollisionState2D
	{
		public bool right;
		public bool left;
		public bool above;
		public bool below;


		public bool hasCollision()
		{
			return below || right || left || above;
		}

	}

	#endregion


	#region events, properties and fields

	public event Action<RaycastHit2D> onControllerCollidedEvent;


	/// <summary>
	/// when true, one way platforms will be ignored when moving vertically for a single frame
	/// </summary>

	[SerializeField]
	[Range( 0.001f, 0.3f )]
	float _skinWidth = 0.02f;

	/// <summary>
	/// defines how far in from the edges of the collider rays are cast from. If cast with a 0 extent it will often result in ray hits that are
	/// not desired (for example a foot collider casting horizontally from directly on the surface can result in a hit)
	/// </summary>
	public float skinWidth
	{
		get { return _skinWidth; }
		set
		{
			_skinWidth = value;
		}
	}


	/// <summary>
	/// mask with all layers that the player should interact with
	/// </summary>
	public LayerMask platformMask = 0;
       
	/// <summary>
	/// mask with all layers that should act as one-way platforms. Note that one-way platforms should always be EdgeCollider2Ds. This is because it does not support being
	/// updated anytime outside of the inspector for now.
	/// </summary>
	[SerializeField]
	LayerMask oneWayPlatformMask = 0;
        

        

	[Range( 2, 20 )]
	public int totalHorizontalRays = 8;
	[Range( 2, 20 )]
	public int totalVerticalRays = 4;

        


	[HideInInspector][NonSerialized]
	public new Transform transform;
	[HideInInspector][NonSerialized]
	public BoxCollider2D boxCollider;
	[HideInInspector][NonSerialized]
	public Rigidbody2D rigidBody2D;

	[HideInInspector][NonSerialized]
	public CharacterCollisionState2D collisionState = new CharacterCollisionState2D();
	[HideInInspector][NonSerialized]
	public Vector3 velocity;

	const float kSkinWidthFloatFudgeFactor = 0.001f;

	#endregion


	/// <summary>
	/// holder for our raycast origin corners (TR, TL, BR, BL)
	/// </summary>
	CharacterRaycastOrigins _raycastOrigins;

	/// <summary>
	/// stores our raycast hit during movement
	/// </summary>
	RaycastHit2D _raycastHit;

	/// <summary>
	/// stores any raycast hits that occur this frame. we have to store them in case we get a hit moving
	/// horizontally and vertically so that we can send the events after all collision state is set
	/// </summary>
	List<RaycastHit2D> _raycastHitsThisFrame = new List<RaycastHit2D>( 2 );

	// horizontal/vertical movement data
	//float _verticalDistanceBetweenRays;
	//float _horizontalDistanceBetweenRays;
        


	#region Monobehaviour

	void Awake()
	{
		// add our one-way platforms to our normal platform mask so that we can land on them from above
		platformMask |= oneWayPlatformMask;

		// cache some components
		transform = GetComponent<Transform>();
		boxCollider = GetComponent<BoxCollider2D>();
		rigidBody2D = GetComponent<Rigidbody2D>();

		// here, we trigger our properties that have setters with bodies
		skinWidth = _skinWidth;

		
	}


	#endregion


	[System.Diagnostics.Conditional( "DEBUG_CC2D_RAYS" )]
	void DrawRay( Vector3 start, Vector3 dir, Color color )
	{
		Debug.DrawRay( start, dir, color );
	}


	#region Public

	/// <summary>
	/// attempts to move the character to position + deltaMovement. Any colliders in the way will cause the movement to
	/// stop when run into.
	/// </summary>
	/// <param name="deltaMovement">Delta movement.</param>
	public void move( Vector3 deltaMovement )
	{

		// clear our state
		//collisionState.reset();
		_raycastHitsThisFrame.Clear();

		//primeRaycastOrigins();
            

		// now we check movement in the horizontal dir
		if( deltaMovement.x != 0f || deltaMovement.y != 0f)
                moveAround(ref deltaMovement);
            //moveHorizontally( ref deltaMovement );

		// next, check movement in the vertical dir
		//if( deltaMovement.y != 0f )
            //moveAround(ref deltaMovement);

            //moveVertically( ref deltaMovement );

            // move then update our state
            transform.Translate( (deltaMovement), Space.World );

		// only calculate velocity if we have a non-zero deltaTime
		if( Time.deltaTime > 0f )
			velocity = deltaMovement / Time.deltaTime;
        

		

		// send off the collision events if we have a listener
		if( onControllerCollidedEvent != null )
		{
			for( var i = 0; i < _raycastHitsThisFrame.Count; i++ )
				onControllerCollidedEvent( _raycastHitsThisFrame[i] );
		}
	}



	#endregion


	#region Movement Methods

	/// <summary>
	/// resets the raycastOrigins to the current extents of the box collider inset by the skinWidth. It is inset
	/// to avoid casting a ray from a position directly touching another collider which results in wonky normal data.
	/// </summary>
	/// <param name="futurePosition">Future position.</param>
	/// <param name="deltaMovement">Delta movement.</param>
	void primeRaycastOrigins()
	{
		// our raycasts need to be fired from the bounds inset by the skinWidth
		var modifiedBounds = boxCollider.bounds;
		modifiedBounds.Expand( -2f * _skinWidth );

		_raycastOrigins.topLeft = new Vector2( modifiedBounds.min.x, modifiedBounds.max.y );
		_raycastOrigins.bottomRight = new Vector2( modifiedBounds.max.x, modifiedBounds.min.y );
		_raycastOrigins.bottomLeft = modifiedBounds.min;
	}

    void moveAround(ref Vector3 deltaMovement)
    {
            var rayDistance = (Mathf.Abs(deltaMovement.y) + (_skinWidth))*16;
            var rayDirection = new Vector2(deltaMovement.x,deltaMovement.y);
            var initialRayOrigin = new Vector2(transform.position.x, transform.position.y);

            // apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
            initialRayOrigin.x += deltaMovement.x;

            // if we are moving up, we should ignore the layers in oneWayPlatformMask
            var mask = platformMask;
            
                var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y);

                DrawRay(ray, rayDirection * rayDistance * 50, Color.red);
                _raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, mask);
           if (_raycastHit)
           {
                // set our new deltaMovement and recalculate the rayDistance taking it into account
                deltaMovement = new Vector3(_raycastHit.point.x - ray.x, _raycastHit.point.y - ray.y, 0.0f);
                rayDistance = Mathf.Abs(deltaMovement.y);

                // remember to remove the skinWidth from our deltaMovement
                if (deltaMovement.y > 0) //isGoingUp
                {
                   deltaMovement.y -= _skinWidth;
                }
                else
                {
                   deltaMovement.y += _skinWidth;
                }

                if (deltaMovement.x > 0) //isGoingRight
                {
                    deltaMovement.x -= _skinWidth;
                }
                else
                {
                    deltaMovement.x += _skinWidth;
                }


                _raycastHitsThisFrame.Add(_raycastHit);
            }
            

        }



	void moveVertically( ref Vector3 deltaMovement )
	{
        //print("moveVertically");
        var isGoingUp = deltaMovement.y > 0;
		var rayDistance = Mathf.Abs( deltaMovement.y ) + _skinWidth;
		var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
		var initialRayOrigin = isGoingUp ? _raycastOrigins.topLeft : _raycastOrigins.bottomLeft;

		// apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
		initialRayOrigin.x += deltaMovement.x;

		// if we are moving up, we should ignore the layers in oneWayPlatformMask
		var mask = platformMask;

		for( var i = 0; i < totalVerticalRays; i++ )
		{
			var ray = new Vector2( initialRayOrigin.x + i, initialRayOrigin.y );

			DrawRay( ray, rayDirection * rayDistance, Color.red );
			_raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, mask );
			if( _raycastHit )
			{
				// set our new deltaMovement and recalculate the rayDistance taking it into account
				deltaMovement.y = _raycastHit.point.y - ray.y;
				rayDistance = Mathf.Abs( deltaMovement.y );

				// remember to remove the skinWidth from our deltaMovement
				if( isGoingUp )
				{
					deltaMovement.y -= _skinWidth;
					collisionState.above = true;
				}
				else
				{
					deltaMovement.y += _skinWidth;
					collisionState.below = true;
				}

				_raycastHitsThisFrame.Add( _raycastHit );
                    

				// we add a small fudge factor for the float operations here. if our rayDistance is smaller
				// than the width + fudge bail out because we have a direct impact
				if( rayDistance < _skinWidth + kSkinWidthFloatFudgeFactor )
					break;
			}
		}
	}

	#endregion

}}