Feature: Scenario new API
  Scenario: File and scenario inference
  Scenario: Adding before after
  Scenario: Use custom extension
    Given foo = bar
    Then foo equals bar

  Scenario: Foo should equal Bar
    Given bar
    When running test
    And doing something
    Then foo equals bar
    And succeeds

  Scenario: Steps can set state
    Given a saved value 10
    When a value 20 is also saved
    Then can add two values from state
    And verify the result