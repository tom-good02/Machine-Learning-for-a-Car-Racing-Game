using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParentSelection
{
    private const float Tolerance = 0.0001f;

    private readonly EASettings _eaSettings;
    public ParentSelection (EASettings eaSettings)
    {
        _eaSettings = eaSettings;
    }
    
    // Roulette Wheel Selection
    // Tournament Selection
    // Returns both parents indexes
    public void SelectParents(out int parentOneIndex, out int parentTwoIndex, List<float> currentGenFitnesses)
    {
        switch (_eaSettings.GetSelectionType())
        {
            case EASettings.SelectionType.Tournament:
                TournamentSelection(out parentOneIndex, out parentTwoIndex, currentGenFitnesses);
                break;
            case EASettings.SelectionType.RouletteWheel:
                RouletteWheelSelection(out parentOneIndex, out parentTwoIndex, currentGenFitnesses);
                break;
            default:
                throw new Exception("Invalid selection type");
        }
    }

    private void TournamentSelection(out int parentOneIndex, out int parentTwoIndex, List<float> currentGenFitnesses)
    {
        TournamentFindParent(out parentOneIndex, currentGenFitnesses);
        TournamentFindParent(out parentTwoIndex, currentGenFitnesses, parentOneIndex);
    }

    private void TournamentFindParent(out int bestTourMember, List<float> currentGenFitnesses, int excludeIndex = -1)
    {
        bestTourMember = -1;
        if(_eaSettings.GetTourSize() > _eaSettings.GetPopulationSize()) 
            _eaSettings.SetTourSize(_eaSettings.GetPopulationSize());
        
        var indexes = new List<int>();
        for (var i = 0; i < _eaSettings.GetPopulationSize(); i++)
        {
            if (i == excludeIndex)
                continue;
            indexes.Add(i);
        }
        
        var bestTourMemberFitness = float.MinValue;
        if (_eaSettings.GetTourSize() == _eaSettings.GetPopulationSize())
        {
            for (var i = 0; i < _eaSettings.GetPopulationSize(); i++)
            {
                if (i == excludeIndex)
                    continue;
                if (currentGenFitnesses[i] > bestTourMemberFitness || (Math.Abs(currentGenFitnesses[i] - bestTourMemberFitness) < Tolerance && Random.Range(0, 2) == 0))
                {
                    bestTourMember = i;
                    bestTourMemberFitness = currentGenFitnesses[i];
                }
            }
            return;
        }
        
        for (var i = 0; i < _eaSettings.GetTourSize(); i++)
        {
            var randomIndex = Random.Range(0, indexes.Count);
            var index = indexes[randomIndex];
        
            if (currentGenFitnesses[index] > bestTourMemberFitness || (Math.Abs(currentGenFitnesses[index] - bestTourMemberFitness) < Tolerance && Random.Range(0, 2) == 0))
            {
                bestTourMember = index;
                bestTourMemberFitness = currentGenFitnesses[index];
            }
            
            indexes.RemoveAt(randomIndex);
        }
        
        if (bestTourMember == -1)
            throw new Exception("Tournament failed to return a parent");
    }

    private void RouletteWheelSelection(out int parentOneIndex, out int parentTwoIndex, List<float> currentGenFitnesses)
    {
        RouletteFindParent(out parentOneIndex, currentGenFitnesses);
        RouletteFindParent(out parentTwoIndex, currentGenFitnesses, parentOneIndex);
    }
    
    private void RouletteFindParent(out int rouletteWheelChoice, List<float> currentGenFitnesses, int excludeIndex = -1)
    {
        // Find the smallest value, if negative add the difference from it to 0 to all values, then add fitnesses together
        // Ensures no negative fitnesses exist but keeps the same distribution
        var smallestFitness = currentGenFitnesses.Min();
        var fitnesses = new List<float>(currentGenFitnesses);
        if (smallestFitness < 0)
        {
            for (var i = 0; i < fitnesses.Count; i++)
            {
                fitnesses[i] += -smallestFitness;
            }
        }

        var totalFitness = fitnesses.Sum();
        
        if (excludeIndex != -1)
            totalFitness -= fitnesses[excludeIndex];
        
        var random = Random.Range(0f, totalFitness);
        
        for(var j = 0; j < _eaSettings.GetPopulationSize(); j++) 
        {
            if (j == excludeIndex || currentGenFitnesses[j] <= 0)
                continue;
            random -= currentGenFitnesses[j];
            if (!(random <= 0)) 
                continue;
            rouletteWheelChoice = j;
            return;
        }   
        
        rouletteWheelChoice = Random.Range(0, _eaSettings.GetPopulationSize());
    }
}
