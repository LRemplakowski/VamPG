using UnityEngine;
using UnityEngine.Profiling;

namespace UMA
{
	/// <summary>
	/// Base class for UMA character.
	/// </summary>
	public abstract class UMAAvatarBase : MonoBehaviour
	{
		public UMAContextBase context;
		[SerializeField]
		protected UMAData _umaData;
		public UMAData UmaData 
		{ 
			get 
			{
                if (_umaData == null)
                {
					_umaData = EnsureUMAData();
                }
                return _umaData;
			}
			set
			{
				_umaData = value;
			}
		}
		public UMARendererAsset defaultRendererAsset; // this can be null if no default renderers need to be applied.

		/// <summary>
		/// The serialized basic UMA recipe.
		/// </summary>
		public UMARecipeBase umaRecipe;
		/// <summary>
		/// Additional partial UMA recipes (not serialized).
		/// </summary>
		public UMARecipeBase[] umaAdditionalRecipes;
		public UMAGeneratorBase umaGenerator;
		public RuntimeAnimatorController animationController;

		protected RaceData umaRace = null;

		/// <summary>
		/// Callback event when character is created.
		/// </summary>
		public UMADataEvent CharacterCreated;
		/// <summary>
		/// Callback event when character is started.
		/// </summary>
		public UMADataEvent CharacterBegun;
		/// <summary>
		/// Callback event when character is destroyed.
		/// </summary>
		public UMADataEvent CharacterDestroyed;
		/// <summary>
		/// Callback event when character is updated.
		/// </summary>
		public UMADataEvent CharacterUpdated;
		/// <summary>
		/// Callback event when character DNA is updated.
		/// </summary>
		public UMADataEvent CharacterDnaUpdated;

		public UMADataEvent AnimatorStateSaved;
		public UMADataEvent AnimatorStateRestored;

		public virtual void Start()
		{
			Initialize();
		}

		private UMAData EnsureUMAData()
		{
			var result = GetComponent<UMAData>();
			if (result == null)
			{
				result = gameObject.AddComponent<UMAData>();
                result.umaRecipe = new UMAData.UMARecipe();
                if (umaGenerator != null && !umaGenerator.gameObject.activeInHierarchy)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogError("Invalid UMA Generator on Avatar.", gameObject);
                        Debug.LogError("UMA generators must be active scene objects!", umaGenerator.gameObject);
                    }
                    umaGenerator = null;
                }
            }
			if (umaGenerator != null)
			{
				result.umaGenerator = umaGenerator;
			}
			return result;
		}

		public void Initialize()
		{
			if (context == null)
			{
				context = UMAContextBase.Instance;
			}

			if (_umaData == null)
			{
				_umaData = EnsureUMAData();
			}
			
			if (CharacterCreated != null) UmaData.CharacterCreated = CharacterCreated;
			if (CharacterBegun != null) UmaData.CharacterBegun = CharacterBegun;
			if (CharacterDestroyed != null) UmaData.CharacterDestroyed = CharacterDestroyed;
			if (CharacterUpdated != null) UmaData.CharacterUpdated = CharacterUpdated;
			if (CharacterDnaUpdated != null) UmaData.CharacterDnaUpdated = CharacterDnaUpdated;
			if (AnimatorStateSaved != null) UmaData.AnimatorStateSaved = AnimatorStateSaved;
			if (AnimatorStateRestored != null) UmaData.AnimatorStateRestored = AnimatorStateRestored;
		}

		/// <summary>
		/// Load a UMA recipe into the avatar.
		/// </summary>
		/// <param name="umaRecipe">UMA recipe.</param>
		public virtual void Load(UMARecipeBase umaRecipe)
		{
			Load(umaRecipe, null);
		}
		/// <summary>
		/// Load a UMA recipe and additional recipes into the avatar.
		/// </summary>
		/// <param name="umaRecipe">UMA recipe.</param>
		/// <param name="umaAdditionalRecipes">Additional recipes.</param>
		public virtual void Load(UMARecipeBase umaRecipe, params UMARecipeBase[] umaAdditionalRecipes)
		{
			if (umaRecipe == null) return;
			if (UmaData == null)
			{
				Initialize();
			}
			Profiler.BeginSample("Load");

			this.umaRecipe = umaRecipe;

			umaRecipe.Load(UmaData.umaRecipe, context);
			UmaData.AddAdditionalRecipes(umaAdditionalRecipes, context);

			if (umaRace != UmaData.umaRecipe.raceData)
			{
				UpdateNewRace();
			}
			else
			{
				UpdateSameRace();
			}
			Profiler.EndSample();
		}

		public void UpdateSameRace()
		{
#if SUPER_LOGGING
			Debug.Log("UpdateSameRace on DynamicCharacterAvatar: " + gameObject.name);
#endif
			if (animationController != null)
			{
				UmaData.animationController = animationController;
			}
			UmaData.Dirty(true, true, true);
		}

		public void UpdateNewRace()
		{
#if SUPER_LOGGING
			Debug.Log("UpdateNewRace on DynamicCharacterAvatar: " + gameObject.name);
#endif

			umaRace = UmaData.umaRecipe.raceData;
			if (animationController != null)
			{
				UmaData.animationController = animationController;
			}

			UmaData.umaGenerator = umaGenerator;

			UmaData.Dirty(true, true, true);
		}

		public virtual void Hide()
		{
			Hide(true);
		}

		/// <summary>
		/// Hide the avatar and clean up its components.
		/// </summary>
		public virtual void Hide(bool DestroyRoot = true)
		{
			if (UmaData != null)
			{
				UmaData.CleanTextures();
				UmaData.CleanMesh(true);
				UmaData.CleanAvatar();
				if (DestroyRoot)
				{
				UMAUtils.DestroySceneObject(UmaData.umaRoot);
				UmaData.umaRoot = null;
					UmaData.skeleton = null;
				}
				UmaData.SetRenderers(null);
				UmaData.SetRendererAssets(null);
				UmaData.animator = null;
				UmaData.firstBake = true;
			}
			umaRace = null;
		}

		/// <summary>
		/// Load the avatar recipe and create components.
		/// </summary>
		public virtual void Show()
		{
			if (umaRecipe != null)
			{
				Load(umaRecipe);
			}
			else
			{
				if (umaRace != UmaData.umaRecipe.raceData)
				{
					UpdateNewRace();
				}
				else
				{
					UpdateSameRace();
				}
			}
		}

		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.white;
			Gizmos.DrawWireCube(transform.position, new Vector3(0.6f, 0.2f, 0.6f));
		}
	}
}
