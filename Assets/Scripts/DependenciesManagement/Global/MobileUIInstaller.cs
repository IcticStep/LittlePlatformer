using Entities.Controls;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Zenject;

namespace DependenciesManagement.Global
{
    public class MobileUIInstaller : MonoInstaller
    {
        [SerializeField] private EventSystem _eventSystemPrefab;
        [SerializeField] private MobileControls _mobileUIControlsPrefab;
        
        public override void InstallBindings()
        {
            #if (UNITY_ANDROID || UNITY_IOS)
                CreateMobileControlUI();
            #endif
        }

        private void CreateMobileControlUI()
        {
            var eventSystem = Container.InstantiatePrefabForComponent<EventSystem>(_eventSystemPrefab);
            DontDestroyOnLoad(eventSystem);
            
            var mobileControls = Container.InstantiatePrefabForComponent<MobileControls>(_mobileUIControlsPrefab);
            Container
                .Bind<MobileControls>()
                .FromInstance(mobileControls)
                .AsSingle();
        }
    }
}